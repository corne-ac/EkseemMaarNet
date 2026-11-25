namespace EkseemMaarNet.Controls;

/// <summary>Breakpoints shared by every page so phones, small tablets and large tablets lay out consistently.</summary>
public static class Responsive
{
    /// <summary>Widest the reading column grows before it centres (keeps line length comfortable on iPads).</summary>
    public const double MaxContentWidth = 1100;

    /// <summary>Home tiles: 2 columns on very narrow phones, 3 on phones, 4 on tablets (12 tiles = an even grid).</summary>
    public static int TileSpan(double width) => width switch
    {
        <= 0 => 3,
        < 340 => 2,
        < 600 => 3,
        _ => 4
    };

    /// <summary>Content cards: one column on phones, two side by side on tablets / landscape.</summary>
    public static int CardSpan(double width) => width >= 720 ? 2 : 1;

    /// <summary>Keeps a grid list's column count in step with its width (rotation, split view, tablets).</summary>
    public static void BindSpan(this CollectionView list, Func<double, int> spanFor) =>
        list.SizeChanged += (_, _) =>
        {
            if (list.ItemsLayout is GridItemsLayout grid && spanFor(list.Width) is var span && grid.Span != span)
                grid.Span = span;
        };
}
