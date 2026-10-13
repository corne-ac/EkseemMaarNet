using EkseemMaarNet.Models;

namespace EkseemMaarNet.Controls;

/// <summary>Explains each rating colour/icon. Built in code so it always matches <see cref="RatingStyle"/>.</summary>
public sealed class RatingLegend : VerticalStackLayout
{
    private static readonly (Rating Rating, string Meaning)[] Rows =
    [
        (Rating.Best, "The strongest evidence, or the most useful. Start here."),
        (Rating.Good, "Likely to help a lot of people."),
        (Rating.Ok, "Generally fine. No need to avoid it unless it bothers you."),
        (Rating.Moderation, "Fine in small amounts, or now and then."),
        (Rating.Caution, "Not much evidence, possible side effects, or check with a doctor first."),
        (Rating.Avoid, "Often makes eczema worse, or can do harm.")
    ];

    public RatingLegend()
    {
        Spacing = 10;
        Build();
        // Rating colours differ between light and dark mode, so rebuild if the theme changes.
        if (Application.Current is { } app)
            app.RequestedThemeChanged += OnThemeChanged;
    }

    private void OnThemeChanged(object? sender, AppThemeChangedEventArgs e) =>
        MainThread.BeginInvokeOnMainThread(Build);

    private void Build()
    {
        Clear();
        foreach (var (rating, meaning) in Rows)
        {
            var grid = new Grid
            {
                ColumnDefinitions = [new ColumnDefinition(26), new ColumnDefinition(GridLength.Star)],
                ColumnSpacing = 10
            };
            grid.Add(new Label
            {
                Text = RatingStyle.Glyph(rating),
                FontFamily = Icons.FontFamily,
                FontSize = 22,
                TextColor = RatingStyle.Colour(rating),
                VerticalOptions = LayoutOptions.Start
            });
            var text = new Label
            {
                FormattedText = new FormattedString
                {
                    Spans =
                    {
                        new Span { Text = RatingStyle.Label(rating), FontFamily = "OpenSansSemibold", TextColor = RatingStyle.Colour(rating) },
                        new Span { Text = ": " + meaning }
                    }
                }
            };
            grid.Add(text, 1);
            Add(grid);
        }
    }
}
