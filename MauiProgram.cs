using EkseemMaarNet.Services;
using EkseemMaarNet.ViewModels;
using EkseemMaarNet.Views;
using Microsoft.Extensions.Logging;

namespace EkseemMaarNet;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Oswald-Regular.ttf", "OswaldRegular");
                fonts.AddFont("Oswald-SemiBold.ttf", "OswaldSemiBold");
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("FluentSystemIcons-Regular.ttf", "FluentIcons");
            });

        RemoveEntryChrome();

        builder.Services.AddSingleton<ContentService>();

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MainViewModel>();

        builder.Services.AddTransient<CategoryPage>();
        builder.Services.AddTransient<CategoryViewModel>();

        builder.Services.AddTransient<GlossaryPage>();
        builder.Services.AddTransient<GlossaryViewModel>();

        builder.Services.AddTransient<AboutPage>();
        builder.Services.AddTransient<AboutViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    // Entries sit inside our own rounded cards, so strip the native border/underline.
    private static void RemoveEntryChrome()
    {
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoChrome", (handler, _) =>
        {
#if IOS
            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#elif ANDROID
            handler.PlatformView.BackgroundTintList =
                Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
        });
    }
}
