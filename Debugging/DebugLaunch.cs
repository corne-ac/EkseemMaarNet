#if DEBUG
using System.Collections.Specialized;
using EkseemMaarNet.ViewModels;

namespace EkseemMaarNet.Debugging;

/// <summary>
/// Debug-only launch options for screenshots and UI checks (not compiled into Release builds).
/// iOS: SIMCTL_CHILD_EMN_START_ROUTE=... xcrun simctl launch; Android: adb shell setprop debug.mono.env "'EMN_START_ROUTE=...'".
/// </summary>
public static class DebugLaunch
{
    private static readonly string? StartRoute = Environment.GetEnvironmentVariable("EMN_START_ROUTE");   // e.g. CategoryPage?id=food
    private static readonly string? StartSearch = Environment.GetEnvironmentVariable("EMN_START_SEARCH"); // e.g. coffee
    private static readonly bool SkipWelcome = Environment.GetEnvironmentVariable("EMN_SKIP_WELCOME") == "1";
    private static readonly string Scroll = Environment.GetEnvironmentVariable("EMN_SCROLL") ?? "";      // "3" top, "3c" centred

    private static bool _homeApplied;

    /// <summary>Runs once, after Home has loaded its content.</summary>
    public static async Task ApplyToHomeAsync(MainViewModel home)
    {
        if (_homeApplied)
            return;
        _homeApplied = true;

        if (SkipWelcome)
            home.ShowWelcome = false;
        if (!string.IsNullOrWhiteSpace(StartSearch))
            home.SearchText = StartSearch;
        if (!string.IsNullOrWhiteSpace(StartRoute))
            await Shell.Current.GoToAsync(StartRoute);
    }

    /// <summary>Scrolls a category page to the requested section as soon as that section exists.</summary>
    public static void ApplyToCategory(CollectionView list, CategoryViewModel category)
    {
        var centre = Scroll.EndsWith('c');
        if (!int.TryParse(Scroll.TrimEnd('c'), out var index) || index <= 0)
            return;

        NotifyCollectionChangedEventHandler? handler = null;
        handler = (_, _) =>
        {
            if (category.Sections.Count <= index)
                return;
            category.Sections.CollectionChanged -= handler;
            list.Dispatcher.Dispatch(() =>
                list.ScrollTo(index, position: centre ? ScrollToPosition.Center : ScrollToPosition.Start, animate: false));
        };
        category.Sections.CollectionChanged += handler;
    }
}
#endif
