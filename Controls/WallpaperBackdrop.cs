namespace EkseemMaarNet.Controls;

/// <summary>
/// The monstera wallpaper, edge to edge, with a dark green scrim on top. Text sits straight on it,
/// so the scrim is strong enough for white text; Home keeps a slightly lighter middle to show the leaves.
/// </summary>
public sealed class WallpaperBackdrop : Grid
{
    public static readonly BindableProperty IsHomeProperty =
        BindableProperty.Create(nameof(IsHome), typeof(bool), typeof(WallpaperBackdrop), false,
            propertyChanged: (b, _, _) => ((WallpaperBackdrop)b).UpdateShade());

    private readonly BoxView _shade = new();

    public WallpaperBackdrop()
    {
        InputTransparent = true;
        CascadeInputTransparent = true;
        SafeAreaEdges = SafeAreaEdges.None;
        Add(new Image { Source = "wallpaper.jpg", Aspect = Aspect.AspectFill });
        Add(_shade);
        UpdateShade();
    }

    public bool IsHome
    {
        get => (bool)GetValue(IsHomeProperty);
        set => SetValue(IsHomeProperty, value);
    }

    private void UpdateShade()
    {
        var (top, middle, bottom, middleAt) = IsHome
            ? ("#B3062A16", "#7A062A16", "#CC062A16", 0.45f)
            : ("#CC062A16", "#8C062A16", "#D1062A16", 0.4f);

        _shade.Background = new LinearGradientBrush(
            [
                new GradientStop(Color.FromArgb(top), 0),
                new GradientStop(Color.FromArgb(middle), middleAt),
                new GradientStop(Color.FromArgb(bottom), 1)
            ],
            new Point(0, 0), new Point(0, 1));
    }
}
