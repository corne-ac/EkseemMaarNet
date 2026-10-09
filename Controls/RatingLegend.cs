using EkseemMaarNet.Models;

namespace EkseemMaarNet.Controls;

/// <summary>Explains each rating colour/icon. Built in code so it always matches <see cref="RatingStyle"/>.</summary>
public sealed class RatingLegend : VerticalStackLayout
{
    private static readonly (Rating Rating, string Meaning)[] Rows =
    [
        (Rating.Best, "Strongest evidence or most useful — start here."),
        (Rating.Good, "Likely to help many people."),
        (Rating.Ok, "Generally fine; no reason to avoid unless it bothers you."),
        (Rating.Moderation, "Fine in small amounts or occasionally."),
        (Rating.Caution, "Weak evidence, possible side-effects, or check with a doctor first."),
        (Rating.Avoid, "Commonly makes eczema worse or can cause harm.")
    ];

    public RatingLegend()
    {
        Spacing = 10;
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
                        new Span { Text = " — " + meaning }
                    }
                }
            };
            grid.Add(text, 1);
            Add(grid);
        }
    }
}
