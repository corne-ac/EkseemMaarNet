using EkseemMaarNet.Controls;
using EkseemMaarNet.ViewModels;

namespace EkseemMaarNet.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _vm;

    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        Tiles.SizeChanged += (_, _) => UpdateTileSpan();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
#if DEBUG
        await DebugStartRouteAsync();
#endif
    }

#if DEBUG
    private static bool _startRouteHandled;

    // Dev aid for screenshots/UI checks: launch with EMN_START_ROUTE="CategoryPage?id=food" (iOS: SIMCTL_CHILD_EMN_START_ROUTE).
    private static async Task DebugStartRouteAsync()
    {
        if (_startRouteHandled)
            return;
        _startRouteHandled = true;

        var route = Environment.GetEnvironmentVariable("EMN_START_ROUTE");
        if (!string.IsNullOrWhiteSpace(route))
            await Shell.Current.GoToAsync(route);
    }
#endif

    private void UpdateTileSpan()
    {
        if (Tiles.ItemsLayout is GridItemsLayout grid)
        {
            var span = Responsive.TileSpan(Tiles.Width);
            if (grid.Span != span)
                grid.Span = span;
        }
    }
}
