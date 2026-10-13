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
    private string _affirmationsCategoryId = "";

    public MainViewModel(ContentService content)
    {
        this.content = content;
        // Newer content downloaded in the background: refresh tiles, footer and any open search.
        content.LibraryUpdated += (_, _) => MainThread.BeginInvokeOnMainThread(async () =>
        {
            await ApplyLibraryAsync();
            if (IsSearching)
                await SearchAsync(SearchText);
        });
    }

    [ObservableProperty]
    public partial IReadOnlyList<Category> Categories { get; set; } = [];

    /// <summary>Replaced as a whole per search, so the list redraws once instead of once per result.</summary>
    [ObservableProperty]
    public partial IReadOnlyList<SearchResult> Results { get; set; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSearching))]
    public partial string SearchText { get; set; } = "";

    /// <summary>False while a search is pending, so "Nothing found" doesn't flash before results arrive.</summary>
    [ObservableProperty]
    public partial bool SearchDone { get; set; }

    [ObservableProperty]
    public partial string LastReviewed { get; set; } = "";

    [ObservableProperty]
    public partial string Disclaimer { get; set; } = "";

    [ObservableProperty]
    public partial string Affirmation { get; set; } = "";

    private const string WelcomeSeenKey = "welcome_v2_seen";

    /// <summary>One-time welcome card explaining this is a family project, not a medical app.</summary>
    [ObservableProperty]
    public partial bool ShowWelcome { get; set; }

    public bool IsSearching => !string.IsNullOrWhiteSpace(SearchText);

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
        Categories = library.Categories;
        LastReviewed = library.LastReviewed;
        Disclaimer = library.Disclaimer;
        await RefreshAffirmationAsync();
    }

    /// <summary>Same affirmation all day, a new one tomorrow. App calls this on resume too.</summary>
    public async Task RefreshAffirmationAsync()
    {
        // The content marks its affirmations list with "style": "affirmations".
        var library = await content.GetLibraryAsync();
        var (category, section) = library.Categories
            .SelectMany(c => c.Sections, (c, s) => (Category: c, Section: s))
            .FirstOrDefault(x => x.Section.IsAffirmations && x.Section.Items.Count > 0);

        _affirmationsCategoryId = category?.Id ?? "";
        Affirmation = section is null ? "" : section.Items[DateTime.Today.DayOfYear % section.Items.Count].Name;
    }

    [RelayCommand]
    private Task OpenAffirmations() =>
        Shell.Current.GoToAsync($"{nameof(CategoryPage)}?id={Uri.EscapeDataString(_affirmationsCategoryId)}");

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

            Results = hits;
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
