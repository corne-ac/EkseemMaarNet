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

    public static Color Colour(Rating r) => r switch
    {
        Rating.Best => Color.FromArgb("#155634"),
        Rating.Good => Color.FromArgb("#3E7D33"),
        Rating.Ok => Color.FromArgb("#5F7A68"),
        Rating.Moderation => Color.FromArgb("#A86A00"),
        Rating.Caution => Color.FromArgb("#C2410C"),
        Rating.Avoid => Color.FromArgb("#B3261E"),
        _ => Colors.Transparent
    };

    public static string Glyph(Rating r) => r switch
    {
        Rating.Best => Icons.Star,
        Rating.Good => Icons.CheckmarkCircle,
        Rating.Ok => Icons.CheckmarkCircle,
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
    public static readonly string Book = FromHex("e179");
    public static readonly string Star = FromHex("f710");
    public static readonly string CheckmarkCircle = FromHex("f299");
    public static readonly string SubtractCircle = FromHex("f7b1");
    public static readonly string Warning = FromHex("f86a");
    public static readonly string DismissCircle = FromHex("f36e");
    public static readonly string Link = FromHex("f4e5");

    public static string FromHex(string hex) =>
        int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var cp)
            ? char.ConvertFromUtf32(cp)
            : "";
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip, AllowTrailingCommas = true)]
[JsonSerializable(typeof(ContentLibrary))]
internal partial class ContentJsonContext : JsonSerializerContext;
