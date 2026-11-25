namespace EkseemMaarNet.Controls;

/// <summary>
/// The monstera wallpaper, edge to edge, with a dark green gradient on top so white text stays readable.
/// Home uses a lighter middle so the tiles show more of the leaves.
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
            ? ("#99062A16", "#33062A16", "#B3062A16", 0.45f)
            : ("#B3062A16", "#4D062A16", "#B3062A16", 0.35f);

        _shade.Background = new LinearGradientBrush(
            [
                new GradientStop(Color.FromArgb(top), 0),
                new GradientStop(Color.FromArgb(middle), middleAt),
                new GradientStop(Color.FromArgb(bottom), 1)
            ],
            new Point(0, 0), new Point(0, 1));
    }
}
