using Microsoft.Extensions.Logging;

namespace AndroidTemperatureChecker;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<Services.ITemperatureService, Platforms.Android.AndroidTemperatureService>();
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}
