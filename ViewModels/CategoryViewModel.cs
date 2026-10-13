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

/// <summary>A section as shown after filtering: the section itself plus only the matching items.</summary>
public sealed record SectionView(ContentSection Section, IReadOnlyList<ContentItem> Items)
{
    public bool HasItems => Items.Count > 0;
}

public partial class CategoryViewModel(ContentService content) : PageViewModel, IQueryAttributable
{
    private static readonly (RatingGroup Group, string Label)[] FilterGroups =
    [
        (RatingGroup.Helpful, "Helpful"),
        (RatingGroup.Careful, "Be careful"),
        (RatingGroup.Avoid, "Avoid")
    ];

    private static readonly Category NotFound = new()
    {
        Title = "Page not found",
        Intro = "This page isn't available any more. Go back and pick another category."
    };

    public ObservableCollection<SectionView> Sections { get; } = [];
    public ObservableCollection<FilterChip> Filters { get; } = [];

    [ObservableProperty]
    public partial Category? Category { get; set; }

    [ObservableProperty]
    public partial bool ShowFilters { get; set; }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        // Shell has already decoded the query value.
        if (query.TryGetValue("id", out var id) && id is string categoryId)
            _ = LoadAsync(categoryId);
    }

    private async Task LoadAsync(string id)
    {
        // e.g. a content update removed this category while it was being opened.
        Category = await content.GetCategoryAsync(id) ?? NotFound;

        // Only offer filters when the page actually mixes "good" and "careful/avoid" items.
        var groups = Category.Sections
            .SelectMany(s => s.Items)
            .Select(i => RatingStyle.Group(i.Rating))
            .Where(g => g != RatingGroup.All)
            .ToHashSet();

        Filters.Clear();
        if (groups.Count > 1)
        {
            Filters.Add(new FilterChip(RatingGroup.All, "All") { IsSelected = true });
            foreach (var (group, label) in FilterGroups.Where(f => groups.Contains(f.Group)))
                Filters.Add(new FilterChip(group, label));
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

    private void ApplyFilter(RatingGroup group)
    {
        Sections.Clear();
        foreach (var section in Category?.Sections ?? [])
        {
            var items = group == RatingGroup.All
                ? section.Items
                : section.Items.Where(i => RatingStyle.Group(i.Rating) == group).ToList();

            // Text-only sections (no items) stay visible under "All" only.
            if (items.Count == 0 && (group != RatingGroup.All || !section.HasIntro))
                continue;

            Sections.Add(new SectionView(section, items));
        }
    }

    [RelayCommand]
    private static Task Home() => Shell.Current.GoToAsync("//MainPage");
}
