using EkseemMaarNet.ViewModels;
using Microsoft.Extensions.Logging;

namespace EkseemMaarNet
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-NanumGothic-Regular.ttf", "NanumGothicRegular");
                    fonts.AddFont("NanumGothic-Bold.ttf", "NanumGothicBold");
                });

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainViewModel>();

            builder.Services.AddTransient<MainFactorsPage>();
            builder.Services.AddTransient<MainFactorsViewModel>();

            builder.Services.AddTransient<GlossaryPage>();
            builder.Services.AddTransient<GlossaryViewModel>();

            builder.Services.AddTransient<WonderCuresPage>();
            builder.Services.AddTransient<WonderCuresViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
