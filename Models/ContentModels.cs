using System.Globalization;
using System.Text.Json.Serialization;

namespace EkseemMaarNet.Models;

/// <summary>How an item rates for someone with eczema. Drives the coloured pill on every item.</summary>
[JsonConverter(typeof(JsonStringEnumConverter<Rating>))]
public enum Rating
{
    Info,
    Best,
    Good,
    Ok,
    Moderation,
    Caution,
    Avoid
}

public sealed class ContentLibrary
{
    /// <summary>Highest content format this build understands. Bump only when the JSON shape changes.</summary>
    public const int SupportedSchemaVersion = 1;

    /// <summary>Format of this file. Apps ignore downloaded content with a newer schema than they support.</summary>
    public int SchemaVersion { get; set; } = 1;

    /// <summary>Increases with every content edit (e.g. 2026101301). The highest version available wins.</summary>
    public long ContentVersion { get; set; }

    public string LastReviewed { get; set; } = "";
    public string Disclaimer { get; set; } = "";
    public List<Category> Categories { get; set; } = [];
    public List<GlossaryEntry> Glossary { get; set; } = [];
}

public sealed class Category
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string TileTitle { get; set; } = "";
    public string Subtitle { get; set; } = "";
    /// <summary>Fluent System Icons codepoint as hex, e.g. "eee9".</summary>
    public string Icon { get; set; } = "";
    public string Intro { get; set; } = "";
    public List<ContentSection> Sections { get; set; } = [];
    public List<SourceLink> Sources { get; set; } = [];

    [JsonIgnore]
    public string Glyph => Icons.FromHex(Icon);

    [JsonIgnore]
    public string DisplayTileTitle => string.IsNullOrEmpty(TileTitle) ? Title : TileTitle;
}

public sealed class ContentSection
{
    public string Title { get; set; } = "";
    public string Intro { get; set; } = "";
    /// <summary>"alert" renders the section as an urgent card (red outline, warning icon).</summary>
    public string Style { get; set; } = "";
    public List<ContentItem> Items { get; set; } = [];

    [JsonIgnore]
    public bool HasIntro => !string.IsNullOrWhiteSpace(Intro);
}

public sealed class ContentItem
{
    public string Name { get; set; } = "";
    public string Note { get; set; } = "";
    public Rating Rating { get; set; } = Rating.Info;
    public List<string> Tags { get; set; } = [];
    public string? Url { get; set; }

    [JsonIgnore] public bool HasNote => !string.IsNullOrWhiteSpace(Note);
    [JsonIgnore] public bool HasTags => Tags.Count > 0;
    [JsonIgnore] public bool HasUrl => !string.IsNullOrWhiteSpace(Url);
    [JsonIgnore] public bool HasRating => Rating != Rating.Info;
    [JsonIgnore] public string RatingLabel => RatingStyle.Label(Rating);
    [JsonIgnore] public Color RatingColor => RatingStyle.Colour(Rating);
    [JsonIgnore] public string RatingGlyph => RatingStyle.Glyph(Rating);
    [JsonIgnore] public string TagText => string.Join("  ·  ", Tags);
    [JsonIgnore] public bool IsPhone => Url?.StartsWith("tel:", StringComparison.OrdinalIgnoreCase) == true;
    [JsonIgnore] public string LinkLabel => IsPhone ? "Call " + FormatPhone(Url![4..]) : "Learn more";

    // South African toll-free style: 0800 567 567. Short codes (10177) stay as they are.
    private static string FormatPhone(string digits) =>
        digits.Length == 10 ? $"{digits[..4]} {digits[4..7]} {digits[7..]}" : digits;
    [JsonIgnore] public string LinkGlyph => IsPhone ? Icons.Call : Icons.Open;
}

public sealed class SourceLink
{
    public string Title { get; set; } = "";
    public string Url { get; set; } = "";
}

public sealed class GlossaryEntry
{
    public string Term { get; set; } = "";
    public string Meaning { get; set; } = "";
    public string Detail { get; set; } = "";

    [JsonIgnore] public bool HasDetail => !string.IsNullOrWhiteSpace(Detail);
}

/// <summary>Filter groups shown as chips on a category page.</summary>
public enum RatingGroup
{
    All,
    Helpful,
    Careful,
    Avoid
}

public static class RatingStyle
{
    public static string Label(Rating r) => r switch
    {
        Rating.Best => "Best option",
        Rating.Good => "Helpful",
        Rating.Ok => "Usually fine",
        Rating.Moderation => "In moderation",
        Rating.Caution => "Caution",
        Rating.Avoid => "Avoid",
        _ => ""
    };

    // Darker tones on white cards; lighter tones on the dark-mode cards so labels keep their contrast.
    public static Color Colour(Rating r) => IsDark ? DarkColour(r) : LightColour(r);

    private static bool IsDark => Application.Current?.RequestedTheme == AppTheme.Dark;

    private static Color LightColour(Rating r) => r switch
    {
        Rating.Best => Color.FromArgb("#155634"),
        Rating.Good => Color.FromArgb("#3E7D33"),
        Rating.Ok => Color.FromArgb("#5F7A68"),
        Rating.Moderation => Color.FromArgb("#A86A00"),
        Rating.Caution => Color.FromArgb("#C2410C"),
        Rating.Avoid => Color.FromArgb("#B3261E"),
        _ => Colors.Transparent
    };

    private static Color DarkColour(Rating r) => r switch
    {
        Rating.Best => Color.FromArgb("#86D98F"),
        Rating.Good => Color.FromArgb("#9BD08F"),
        Rating.Ok => Color.FromArgb("#B5C7BA"),
        Rating.Moderation => Color.FromArgb("#F2B84B"),
        Rating.Caution => Color.FromArgb("#FFA163"),
        Rating.Avoid => Color.FromArgb("#FF8A80"),
        _ => Colors.Transparent
    };

    public static string Glyph(Rating r) => r switch
    {
        Rating.Best => Icons.Star,
        Rating.Good => Icons.CheckmarkCircle,
        Rating.Ok => Icons.ThumbLike,
        Rating.Moderation => Icons.SubtractCircle,
        Rating.Caution => Icons.Warning,
        Rating.Avoid => Icons.DismissCircle,
        _ => Icons.Info
    };

    public static RatingGroup Group(Rating r) => r switch
    {
        Rating.Best or Rating.Good or Rating.Ok => RatingGroup.Helpful,
        Rating.Moderation or Rating.Caution => RatingGroup.Careful,
        Rating.Avoid => RatingGroup.Avoid,
        _ => RatingGroup.All
    };
}

/// <summary>Glyphs from the Fluent UI System Icons font (MIT, Microsoft).</summary>
public static class Icons
{
    public const string FontFamily = "FluentIcons";

    public static readonly string Back = FromHex("f15c");
    public static readonly string Search = FromHex("f690");
    public static readonly string Dismiss = FromHex("f36a");
    public static readonly string Open = FromHex("f583");
    public static readonly string ChevronRight = FromHex("f2b1");
    public static readonly string Info = FromHex("f4a4");
    public static readonly string Book = FromHex("e174");
    public static readonly string Star = FromHex("f710");
    // The 24px check-circle glyph is malformed in this font build; the 20px variant renders correctly.
    public static readonly string CheckmarkCircle = FromHex("f298");
    public static readonly string ThumbLike = FromHex("f820");
    public static readonly string SubtractCircle = FromHex("f7b1");
    public static readonly string Warning = FromHex("f86a");
    public static readonly string DismissCircle = FromHex("f36e");
    public static readonly string Link = FromHex("f4e5");
    public static readonly string Call = FromHex("e271");
    public static readonly string Sparkle = FromHex("eb34");

    public static string FromHex(string hex) =>
        int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var cp)
            ? char.ConvertFromUtf32(cp)
            : "";
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip, AllowTrailingCommas = true)]
[JsonSerializable(typeof(ContentLibrary))]
internal partial class ContentJsonContext : JsonSerializerContext;
