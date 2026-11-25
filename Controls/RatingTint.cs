using EkseemMaarNet.Models;

namespace EkseemMaarNet.Controls;

/// <summary>
/// Attached property that colours a Label or Span for a rating, e.g. controls:RatingTint.Rating="{Binding Rating}".
/// Uses SetAppThemeColor, so the colour follows light/dark mode without any manual refresh.
/// </summary>
public static class RatingTint
{
    public static readonly BindableProperty RatingProperty =
        BindableProperty.CreateAttached("Rating", typeof(Rating), typeof(RatingTint), Rating.Info, propertyChanged: OnRatingChanged);

    public static Rating GetRating(BindableObject view) => (Rating)view.GetValue(RatingProperty);

    public static void SetRating(BindableObject view, Rating value) => view.SetValue(RatingProperty, value);

    private static void OnRatingChanged(BindableObject view, object oldValue, object newValue)
    {
        var (light, dark) = RatingStyle.Colours((Rating)newValue);
        switch (view)
        {
            case Label label:
                label.SetAppThemeColor(Label.TextColorProperty, light, dark);
                break;
            case Span span:
                span.SetAppThemeColor(Span.TextColorProperty, light, dark);
                break;
        }
    }
}
