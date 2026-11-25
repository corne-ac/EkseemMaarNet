using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.View;

namespace EkseemMaarNet;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // The app draws the dark leaf wallpaper behind the system bars, so use light status/nav bar icons.
        if (Window is { } window && WindowCompat.GetInsetsController(window, window.DecorView) is { } insets)
        {
            insets.AppearanceLightStatusBars = false;
            insets.AppearanceLightNavigationBars = false;
        }
    }
}
