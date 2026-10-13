namespace EkseemMaarNet.Controls;

/// <summary>
/// Wraps one scrolling list and makes its content fade to transparent along the top edge,
/// so items dissolve into the wallpaper as they slide under the page header.
/// The fade is off at rest and strengthens as the user scrolls.
/// Native masking lives in Platforms/iOS and Platforms/Android (FadeHost.*.cs).
/// </summary>
public partial class FadeHost : ContentView
{
    public static readonly BindableProperty FadeHeightProperty =
        BindableProperty.Create(nameof(FadeHeight), typeof(double), typeof(FadeHost), 48d,
            propertyChanged: (b, _, _) => ((FadeHost)b).UpdateNative());

    public static readonly BindableProperty FadeAmountProperty =
        BindableProperty.Create(nameof(FadeAmount), typeof(double), typeof(FadeHost), 0d,
            propertyChanged: (b, _, _) => ((FadeHost)b).UpdateNative());

    public FadeHost()
    {
        SizeChanged += (_, _) => UpdateNative();
        HandlerChanged += (_, _) => UpdateNative();
    }

    /// <summary>Height of the fade band in device-independent units.</summary>
    public double FadeHeight
    {
        get => (double)GetValue(FadeHeightProperty);
        set => SetValue(FadeHeightProperty, value);
    }

    /// <summary>0 = no fade, 1 = full fade. Driven by the scroll offset.</summary>
    public double FadeAmount
    {
        get => (double)GetValue(FadeAmountProperty);
        set => SetValue(FadeAmountProperty, value);
    }

    // The host wraps exactly one scrolling view; follow its offset.
    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName != nameof(Content))
            return;

        switch (Content)
        {
            case ItemsView list:
                list.Scrolled += (_, e) => FadeAmount = AmountFor(e.VerticalOffset);
                break;
            case ScrollView scroll:
                scroll.Scrolled += (_, e) => FadeAmount = AmountFor(e.ScrollY);
                break;
        }
    }

    // Rounded so scrolling only touches the native layer when the fade visibly changes
    // (once it reaches 1, further scrolling changes nothing).
    private double AmountFor(double offset) =>
        FadeHeight <= 0 ? 0 : Math.Round(Math.Clamp(offset / FadeHeight, 0, 1), 2);

    partial void UpdateNative();
}
