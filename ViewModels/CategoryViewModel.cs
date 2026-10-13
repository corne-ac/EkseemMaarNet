using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EkseemMaarNet.Models;
using EkseemMaarNet.Services;

namespace EkseemMaarNet.ViewModels;

/// <summary>A filter chip on the category page.</summary>
public partial class FilterChip(RatingGroup group, string label) : ObservableObject
{
    public RatingGroup Group { get; } = group;
    public string Label { get; } = label;

    [ObservableProperty]
    public partial bool IsSelected { get; set; }
}

/// <summary>A section as shown after filtering (only the matching items).</summary>
public sealed record SectionView(string Title, string Intro, IReadOnlyList<ContentItem> Items, bool IsAlert)
{
    public bool HasIntro => !string.IsNullOrWhiteSpace(Intro);
    public bool IsNormal => !IsAlert;
    public bool HasItems => Items.Count > 0;
}

public partial class CategoryViewModel : PageViewModel, IQueryAttributable
{
    private readonly ContentService _content;
    private Category? _category;
    private RatingGroup _group = RatingGroup.All;

    public CategoryViewModel(ContentService content)
    {
        _content = content;
        // Rating colours are picked per theme, so redraw the cards if light/dark mode changes while the page is open.
        if (Application.Current is { } app)
            app.RequestedThemeChanged += OnThemeChanged;
    }

    public ObservableCollection<SectionView> Sections { get; } = [];
    public ObservableCollection<FilterChip> Filters { get; } = [];

    [ObservableProperty]
    public partial string Title { get; set; } = "";

    [ObservableProperty]
    public partial string Subtitle { get; set; } = "";

    [ObservableProperty]
    public partial string Glyph { get; set; } = "";

    [ObservableProperty]
    public partial string Intro { get; set; } = "";

    [ObservableProperty]
    public partial bool ShowFilters { get; set; }

    [ObservableProperty]
    public partial bool HasSources { get; set; }

    public ObservableCollection<SourceLink> Sources { get; } = [];

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        // Shell has already decoded the query value.
        if (query.TryGetValue("id", out var id) && id is string categoryId)
            _ = LoadAsync(categoryId);
    }

    private async Task LoadAsync(string id)
    {
        _category = await _content.GetCategoryAsync(id);
        if (_category is null)
        {
            // e.g. a content update removed this category while it was being opened.
            Title = "Page not found";
            Intro = "This page isn't available any more. Go back and pick another category.";
            return;
        }

        Title = _category.Title;
        Subtitle = _category.Subtitle;
        Glyph = _category.Glyph;
        Intro = _category.Intro;

        Sources.Clear();
        foreach (var source in _category.Sources)
            Sources.Add(source);
        HasSources = Sources.Count > 0;

        // Only offer filters when the page actually mixes "good" and "careful/avoid" items.
        var groups = _category.Sections
            .SelectMany(s => s.Items)
            .Select(i => RatingStyle.Group(i.Rating))
            .Where(g => g != RatingGroup.All)
            .Distinct()
            .ToHashSet();

        Filters.Clear();
        if (groups.Count > 1)
        {
            Filters.Add(new FilterChip(RatingGroup.All, "All") { IsSelected = true });
            if (groups.Contains(RatingGroup.Helpful)) Filters.Add(new FilterChip(RatingGroup.Helpful, "Helpful"));
            if (groups.Contains(RatingGroup.Careful)) Filters.Add(new FilterChip(RatingGroup.Careful, "Be careful"));
            if (groups.Contains(RatingGroup.Avoid)) Filters.Add(new FilterChip(RatingGroup.Avoid, "Avoid"));
        }
        ShowFilters = Filters.Count > 0;

        ApplyFilter(RatingGroup.All);
    }

    [RelayCommand]
    private void SelectFilter(FilterChip chip)
    {
        foreach (var f in Filters)
            f.IsSelected = f == chip;
        ApplyFilter(chip.Group);
    }

    private void OnThemeChanged(object? sender, AppThemeChangedEventArgs e) =>
        MainThread.BeginInvokeOnMainThread(() => ApplyFilter(_group));

    private void ApplyFilter(RatingGroup group)
    {
        _group = group;
        if (_category is null)
            return;

        Sections.Clear();
        foreach (var section in _category.Sections)
        {
            var items = group == RatingGroup.All
                ? section.Items
                : section.Items.Where(i => RatingStyle.Group(i.Rating) == group).ToList();

            // Text-only sections (no items) stay visible under "All" only.
            if (items.Count == 0 && (group != RatingGroup.All || !section.HasIntro))
                continue;

            var isAlert = string.Equals(section.Style, "alert", StringComparison.OrdinalIgnoreCase);
            Sections.Add(new SectionView(section.Title, section.Intro, items, isAlert));
        }
    }

    [RelayCommand]
    private static Task Home() => Shell.Current.GoToAsync("//MainPage");
}
