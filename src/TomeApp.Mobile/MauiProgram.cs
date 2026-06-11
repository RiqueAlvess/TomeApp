using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using TomeApp.Mobile.Services;
using TomeApp.Mobile.ViewModels;

namespace TomeApp.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Inter-Regular.ttf", "Inter");
                fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
                fonts.AddFont("PlayfairDisplay-Bold.ttf", "PlayfairBold");
            });

        builder.Services.AddSingleton<LocalDatabaseService>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddSingleton<SyncService>();

        builder.Services.AddTransient<LibraryViewModel>();
        builder.Services.AddTransient<TimerViewModel>();
        builder.Services.AddTransient<RankingViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();

        builder.Services.AddTransient<Views.LibraryPage>();
        builder.Services.AddTransient<Views.TimerPage>();
        builder.Services.AddTransient<Views.RankingPage>();
        builder.Services.AddTransient<Views.ProfilePage>();
        builder.Services.AddTransient<Views.AddBookPage>();
        builder.Services.AddTransient<Views.LoginPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
