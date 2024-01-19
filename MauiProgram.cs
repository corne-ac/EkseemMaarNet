﻿using EkseemMaarNet.ViewModels;
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

            builder.Services.AddSingleton<MainFactorsPage>();
            builder.Services.AddSingleton<MainFactorsViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}