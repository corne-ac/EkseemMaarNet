namespace EkseemMaarNet.Controls;

/// <summary>
/// Wraps a scrolling list and makes its content fade to transparent along the top edge,
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

    private readonly Dictionary<VisualElement, double> _offsets = [];

    public FadeHost()
    {
        SizeChanged += (_, _) => UpdateNative();
        HandlerChanged += (_, _) =>
        {
            TrackScrollers();
            UpdateNative();
        };
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

    // Track every vertical list/scroll view inside (Home has two lists: tiles and search results).
    private void TrackScrollers()
    {
        if (Content is null)
            return;

        foreach (var element in Content.GetVisualTreeDescendants().Prepend(Content).OfType<VisualElement>())
        {
            if (_offsets.ContainsKey(element))
                continue;

            switch (element)
            {
                case ItemsView list:
                    _offsets[list] = 0;
                    list.Scrolled += (_, e) => OnScrolled(list, e.VerticalOffset);
                    list.PropertyChanged += OnScrollerPropertyChanged;
                    break;
                case ScrollView scroll when scroll.Orientation != ScrollOrientation.Horizontal:
                    _offsets[scroll] = 0;
                    scroll.Scrolled += (_, e) => OnScrolled(scroll, e.ScrollY);
                    scroll.PropertyChanged += OnScrollerPropertyChanged;
                    break;
            }
        }
    }

    private void OnScrolled(VisualElement scroller, double offset)
    {
        _offsets[scroller] = offset;
        if (scroller.IsVisible)
            FadeAmount = AmountFor(offset);
    }

    // When Home swaps tiles for search results, take the fade from whichever list is now showing.
    private void OnScrollerPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsVisible) && sender is VisualElement { IsVisible: true } scroller)
            FadeAmount = AmountFor(_offsets.GetValueOrDefault(scroller));
    }

    private double AmountFor(double offset) =>
        FadeHeight <= 0 ? 0 : Math.Clamp(offset / FadeHeight, 0, 1);

    partial void UpdateNative();
}
