using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EkseemMaarNet.Models;
using EkseemMaarNet.Services;
using EkseemMaarNet.Views;

namespace EkseemMaarNet.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ContentService content;
    private CancellationTokenSource? _searchCts;

    public MainViewModel(ContentService content)
    {
        this.content = content;
        // Newer content downloaded in the background: refresh tiles, footer and any open search.
        content.LibraryUpdated += (_, _) => MainThread.BeginInvokeOnMainThread(async () =>
        {
            await ApplyLibraryAsync();
            await RefreshSearchAsync();
        });
        // Result cards use theme-specific rating colours, so rebuild them when light/dark mode changes.
        if (Application.Current is { } app)
            app.RequestedThemeChanged += (_, _) => MainThread.BeginInvokeOnMainThread(async () => await RefreshSearchAsync());
    }

    public ObservableCollection<Category> Categories { get; } = [];
    public ObservableCollection<SearchResult> Results { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSearching), nameof(ShowNoResults))]
    public partial string SearchText { get; set; } = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowNoResults))]
    public partial bool HasResults { get; set; }

    /// <summary>False while a search is pending, so "Nothing found" doesn't flash before results arrive.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowNoResults))]
    public partial bool SearchDone { get; set; }

    [ObservableProperty]
    public partial string LastReviewed { get; set; } = "";

    [ObservableProperty]
    public partial string Disclaimer { get; set; } = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasAffirmation))]
    public partial string Affirmation { get; set; } = "";

    public bool HasAffirmation => !string.IsNullOrWhiteSpace(Affirmation);

    private const string WelcomeSeenKey = "welcome_v2_seen";

    /// <summary>One-time welcome card explaining this is a family project, not a medical app.</summary>
    [ObservableProperty]
    public partial bool ShowWelcome { get; set; }

    public bool IsSearching => !string.IsNullOrWhiteSpace(SearchText);
    public bool ShowNoResults => IsSearching && SearchDone && !HasResults;

    public async Task LoadAsync()
    {
        if (Categories.Count > 0)
            return;

        await ApplyLibraryAsync();
        ShowWelcome = !Preferences.Default.Get(WelcomeSeenKey, false);
    }

    private async Task ApplyLibraryAsync()
    {
        var library = await content.GetLibraryAsync();

        Categories.Clear();
        foreach (var category in library.Categories)
            Categories.Add(category);

        LastReviewed = library.LastReviewed;
        Disclaimer = library.Disclaimer;
        Affirmation = PickDailyAffirmation(library);
    }

    /// <summary>Called when the app comes back to the foreground, so a new day shows a new affirmation.</summary>
    public async Task RefreshAffirmationAsync() =>
        Affirmation = PickDailyAffirmation(await content.GetLibraryAsync());

    private Task RefreshSearchAsync() => IsSearching ? SearchAsync(SearchText) : Task.CompletedTask;

    // Same affirmation all day, a new one tomorrow.
    private static string PickDailyAffirmation(ContentLibrary library)
    {
        var affirmations = library.Categories
            .FirstOrDefault(c => c.Id == "motivation")?
            .Sections.FirstOrDefault(s => s.Title.Contains("affirmation", StringComparison.OrdinalIgnoreCase))?
            .Items;
        return affirmations is { Count: > 0 }
            ? affirmations[DateTime.Today.DayOfYear % affirmations.Count].Name
            : "";
    }

    [RelayCommand]
    private Task OpenMotivation() =>
        Shell.Current.GoToAsync($"{nameof(CategoryPage)}?id=motivation");

    [RelayCommand]
    private void DismissWelcome()
    {
        Preferences.Default.Set(WelcomeSeenKey, true);
        ShowWelcome = false;
    }

    partial void OnSearchTextChanged(string value)
    {
        SearchDone = false;
        _ = SearchAsync(value);
    }

    private async Task SearchAsync(string query)
    {
        _searchCts?.Cancel();
        var cts = _searchCts = new CancellationTokenSource();

        try
        {
            await Task.Delay(180, cts.Token); // debounce while typing
            var hits = await content.SearchAsync(query);
            if (cts.IsCancellationRequested)
                return;

            Results.Clear();
            foreach (var hit in hits)
                Results.Add(hit);
            HasResults = Results.Count > 0;
            SearchDone = true;
        }
        catch (TaskCanceledException)
        {
        }
    }

    [RelayCommand]
    private void ClearSearch() => SearchText = "";

    [RelayCommand]
    private Task OpenCategory(Category category) =>
        Shell.Current.GoToAsync($"{nameof(CategoryPage)}?id={Uri.EscapeDataString(category.Id)}");

    [RelayCommand]
    private Task OpenResult(SearchResult result) => OpenCategory(result.Category);

    [RelayCommand]
    private Task Glossary() => Shell.Current.GoToAsync(nameof(GlossaryPage));

    [RelayCommand]
    private Task About() => Shell.Current.GoToAsync(nameof(AboutPage));
}
