using System.Text.Json;
using EkseemMaarNet.Models;

namespace EkseemMaarNet.Services;

/// <summary>A single search hit: the item plus the category it lives in.</summary>
public sealed record SearchResult(ContentItem Item, Category Category, string SectionTitle);

/// <summary>Loads the bundled eczema content (Resources/Raw/content.json) once and serves it to the pages.</summary>
public sealed class ContentService
{
    private Task<ContentLibrary>? _loading;

    public Task<ContentLibrary> GetLibraryAsync() => _loading ??= LoadAsync();

    public async Task<Category?> GetCategoryAsync(string id)
    {
        var library = await GetLibraryAsync();
        return library.Categories.FirstOrDefault(c => string.Equals(c.Id, id, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IReadOnlyList<SearchResult>> SearchAsync(string query, int max = 60)
    {
        var terms = query.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (terms.Length == 0)
            return [];

        var library = await GetLibraryAsync();
        var results = new List<(SearchResult Result, int Score)>();

        foreach (var category in library.Categories)
        foreach (var section in category.Sections)
        foreach (var item in section.Items)
        {
            var score = Score(item, terms);
            if (score > 0)
                results.Add((new SearchResult(item, category, section.Title), score));
        }

        return results
            .OrderByDescending(r => r.Score)
            .ThenBy(r => r.Result.Item.Name, StringComparer.OrdinalIgnoreCase)
            .Take(max)
            .Select(r => r.Result)
            .ToList();
    }

    // Name matches weigh more than note matches so "coffee" finds Coffee before items that mention coffee.
    private static int Score(ContentItem item, string[] terms)
    {
        var total = 0;
        foreach (var term in terms)
        {
            if (item.Name.StartsWith(term, StringComparison.OrdinalIgnoreCase))
                total += 10;
            else if (item.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
                total += 6;
            else if (item.Tags.Any(t => t.Equals(term, StringComparison.OrdinalIgnoreCase)))
                total += 4;
            else if (item.Note.Contains(term, StringComparison.OrdinalIgnoreCase))
                total += 1;
            else
                return 0; // every term must match somewhere
        }
        return total;
    }

    private static async Task<ContentLibrary> LoadAsync()
    {
        await using var stream = await FileSystem.OpenAppPackageFileAsync("content.json");
        var library = await JsonSerializer.DeserializeAsync(stream, ContentJsonContext.Default.ContentLibrary);
        return library ?? new ContentLibrary();
    }
}
