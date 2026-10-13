using System.Net;
using System.Text.Json;
using EkseemMaarNet.Models;

namespace EkseemMaarNet.Services;

/// <summary>A single search hit: the item plus the category it lives in.</summary>
public sealed record SearchResult(ContentItem Item, Category Category, string SectionTitle);

/// <summary>
/// Serves the eczema content to the pages.
///
/// The app opens straight away with the best local copy: either the content.json bundled in the app,
/// or a newer copy downloaded earlier. In the background it checks GitHub (at most once an hour) and, if
/// main has a higher contentVersion that this build can read, saves it and raises <see cref="LibraryUpdated"/>.
/// That means content fixes ship by merging content.json to main, without publishing a new app version.
/// </summary>
public sealed class ContentService
{
    public const string RemoteUrl = "https://raw.githubusercontent.com/corne-ac/EkseemMaarNet/main/Resources/Raw/content.json";
    private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(1);

    private const string FileName = "content.json";
    private const string ETagKey = "content_etag";
    private const string LastCheckKey = "content_last_check";

    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(15) };

    private Task<ContentLibrary>? _loading;
    private ContentLibrary? _current;
    private int _checking;

    /// <summary>Raised (on a background thread) after newer content has been downloaded and applied.</summary>
    public event EventHandler? LibraryUpdated;

    private static string CachePath => Path.Combine(FileSystem.AppDataDirectory, FileName);

#if DEBUG
    // Dev aid: EMN_CONTENT_URL points the app at another copy (e.g. a branch's raw file) to test content before merging.
    private static string SourceUrl => Environment.GetEnvironmentVariable("EMN_CONTENT_URL") is { Length: > 0 } url ? url : RemoteUrl;
#else
    private static string SourceUrl => RemoteUrl;
#endif

    // Loaded off the UI thread: reading and parsing ~100 KB of JSON shouldn't hold up the first frame.
    public async Task<ContentLibrary> GetLibraryAsync() => _current ??= await (_loading ??= Task.Run(LoadLocalAsync));

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

    /// <summary>Checks GitHub for newer content. Safe to call often: throttled, and failures are ignored.</summary>
    public async Task CheckForUpdatesAsync()
    {
        if (Interlocked.Exchange(ref _checking, 1) == 1)
            return;

        try
        {
            var lastCheck = new DateTime(Preferences.Default.Get(LastCheckKey, 0L), DateTimeKind.Utc);
            if (DateTime.UtcNow - lastCheck < CheckInterval)
                return;
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return;

            using var request = new HttpRequestMessage(HttpMethod.Get, SourceUrl);
            var etag = Preferences.Default.Get(ETagKey, "");
            if (!string.IsNullOrEmpty(etag))
                request.Headers.TryAddWithoutValidation("If-None-Match", etag);

            using var response = await Http.SendAsync(request).ConfigureAwait(false);
            Preferences.Default.Set(LastCheckKey, DateTime.UtcNow.Ticks);

            if (response.StatusCode == HttpStatusCode.NotModified || !response.IsSuccessStatusCode)
                return;

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var remote = Parse(json);
            if (remote is null)
                return;

            var current = await GetLibraryAsync().ConfigureAwait(false);
            if (remote.ContentVersion > current.ContentVersion)
            {
                await SaveCacheAsync(json).ConfigureAwait(false);
                _current = remote;
                LibraryUpdated?.Invoke(this, EventArgs.Empty);
            }

            // Only now that we hold this file (or something newer) is "not modified" a safe answer next time.
            if (response.Headers.ETag is { } newTag)
                Preferences.Default.Set(ETagKey, newTag.ToString());
        }
        catch (Exception)
        {
            // Offline, timeout or a bad file: keep using what we have.
        }
        finally
        {
            Interlocked.Exchange(ref _checking, 0);
        }
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

    // Bundled copy vs. previously downloaded copy: whichever has the higher contentVersion wins.
    private static async Task<ContentLibrary> LoadLocalAsync()
    {
        ContentLibrary bundled;
        await using (var stream = await FileSystem.OpenAppPackageFileAsync(FileName))
            bundled = await JsonSerializer.DeserializeAsync(stream, ContentJsonContext.Default.ContentLibrary) ?? new ContentLibrary();

        try
        {
            if (File.Exists(CachePath) && Parse(await File.ReadAllTextAsync(CachePath)) is { } cached
                && cached.ContentVersion > bundled.ContentVersion)
                return cached;
        }
        catch (Exception)
        {
            // A damaged cache file just falls back to the bundled content.
        }

        // Without a usable cache, forget the ETag so the next check downloads the file again.
        Preferences.Default.Remove(ETagKey);

        return bundled;
    }

    /// <summary>Returns null if the JSON is unreadable, from a newer schema, or missing the basics.</summary>
    private static ContentLibrary? Parse(string json)
    {
        if (JsonSerializer.Deserialize(json, ContentJsonContext.Default.ContentLibrary) is not { } library)
            return null;

        Normalize(library);
        return library.SchemaVersion <= ContentLibrary.SupportedSchemaVersion
               && library.Categories.Count > 0
               && library.Categories.All(c => !string.IsNullOrWhiteSpace(c.Id) && !string.IsNullOrWhiteSpace(c.Title))
            ? library
            : null;
    }

    // An explicit JSON null overrides the "" / [] defaults, so fill those back in before anything reads them.
    // Also drops links content isn't allowed to open: items may link to https pages or tel: numbers, sources to https only.
    private static void Normalize(ContentLibrary library)
    {
        library.Categories ??= [];
        library.Glossary ??= [];
        library.Categories.RemoveAll(c => c is null);
        library.Glossary.RemoveAll(g => g is null || string.IsNullOrWhiteSpace(g.Term));
        foreach (var category in library.Categories)
        {
            category.Sections ??= [];
            category.Sources ??= [];
            category.Sections.RemoveAll(s => s is null);
            category.Sources.RemoveAll(s => !LinkRules.IsWeb(s?.Url));
            foreach (var section in category.Sections)
            {
                section.Items ??= [];
                section.Items.RemoveAll(i => i is null || string.IsNullOrWhiteSpace(i.Name));
                foreach (var item in section.Items)
                {
                    item.Note ??= "";
                    item.Tags ??= [];
                    if (!LinkRules.IsAllowed(item.Url))
                        item.Url = null;
                }
            }
        }
    }

    private static async Task SaveCacheAsync(string json)
    {
        var temp = CachePath + ".tmp";
        await File.WriteAllTextAsync(temp, json).ConfigureAwait(false);
        File.Move(temp, CachePath, overwrite: true);
    }
}
