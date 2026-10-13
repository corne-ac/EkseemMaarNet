using EkseemMaarNet.Controls;
using EkseemMaarNet.ViewModels;

namespace EkseemMaarNet.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _vm;
    private bool _resumeHooked;

    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        Tiles.BindSpan(Responsive.TileSpan);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Coming back to the app on a new day should show a new affirmation.
        if (!_resumeHooked && Window is { } window)
        {
            window.Resumed += async (_, _) => await _vm.RefreshAffirmationAsync();
            _resumeHooked = true;
        }

        await _vm.LoadAsync();
#if DEBUG
        await DebugStartRouteAsync();
#endif
    }

#if DEBUG
    private static bool _startRouteHandled;

    // Dev aid for screenshots/UI checks: launch with EMN_START_ROUTE="CategoryPage?id=food" (iOS: SIMCTL_CHILD_EMN_START_ROUTE); EMN_START_SEARCH="coffee" pre-fills search.
    private async Task DebugStartRouteAsync()
    {
        if (_startRouteHandled)
            return;
        _startRouteHandled = true;

        if (Environment.GetEnvironmentVariable("EMN_SKIP_WELCOME") == "1")
            _vm.ShowWelcome = false;

        var search = Environment.GetEnvironmentVariable("EMN_START_SEARCH");
        if (!string.IsNullOrWhiteSpace(search))
            _vm.SearchText = search;

        var route = Environment.GetEnvironmentVariable("EMN_START_ROUTE");
        if (!string.IsNullOrWhiteSpace(route))
            await Shell.Current.GoToAsync(route);
    }
#endif
}
