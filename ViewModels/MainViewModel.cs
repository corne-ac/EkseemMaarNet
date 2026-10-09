using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EkseemMaarNet.Models;
using EkseemMaarNet.Services;
using EkseemMaarNet.Views;

namespace EkseemMaarNet.ViewModels;

public partial class MainViewModel(ContentService content) : ObservableObject
{
    private CancellationTokenSource? _searchCts;

    public ObservableCollection<Category> Categories { get; } = [];
    public ObservableCollection<SearchResult> Results { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSearching), nameof(ShowNoResults))]
    public partial string SearchText { get; set; } = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowNoResults))]
    public partial bool HasResults { get; set; }

    [ObservableProperty]
    public partial string LastReviewed { get; set; } = "";

    [ObservableProperty]
    public partial string Disclaimer { get; set; } = "";

    public bool IsSearching => !string.IsNullOrWhiteSpace(SearchText);
    public bool ShowNoResults => IsSearching && !HasResults;

    public async Task LoadAsync()
    {
        if (Categories.Count > 0)
            return;

        var library = await content.GetLibraryAsync();
        foreach (var category in library.Categories)
            Categories.Add(category);

        LastReviewed = library.LastReviewed;
        Disclaimer = library.Disclaimer;
    }

    partial void OnSearchTextChanged(string value) => _ = SearchAsync(value);

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
