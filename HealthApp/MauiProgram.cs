using CommunityToolkit.Maui;
using HealthApp.FatSecretAPI;
using HealthApp.FirestoreDatabase;
using Microsoft.Extensions.Logging;
using Serilog;
using Syncfusion.Maui.Toolkit.Hosting;

namespace HealthApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureMauiHandlers(handlers =>
                {
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                });

#if DEBUG
            builder.Logging.AddDebug();
            builder.Services.AddLogging(configure => configure.AddDebug());
#endif
            builder.Services.AddSerilog(new LoggerConfiguration()
                .WriteTo.Debug()
                .WriteTo.File(Path.Combine(FileSystem.Current.AppDataDirectory, "Log.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger());

            builder.Services.AddFatSecretAPI();
            builder.Services.RegisterDatabaseServices();

            builder.Services.AddSingleton<MainPageModel>();
            builder.Services.AddSingleton<HealthService>();

            // Food-related pages
            builder.Services.AddTransientWithShellRoute<FoodMainPage, FoodMainPageModel>("food");
            builder.Services.AddTransientWithShellRoute<FoodAddMealPage, FoodAddMealPageModel>("foodAddMeal");
            builder.Services.AddTransientWithShellRoute<FoodPortionDetailsPage, FoodPortionDetailsPageModel>("foodPortionDetails");
            builder.Services.AddTransientWithShellRoute<FoodMealDetailsPage, FoodMealDetailsPageModel>("foodMealDetails");

            // Sleep-related pages
            builder.Services.AddTransientWithShellRoute<SleepMainPage, SleepMainPageModel>("sleep");
            builder.Services.AddTransientWithShellRoute<SleepAddDataPage, SleepAddDataPageModel>("sleepAddData");
            builder.Services.AddTransientWithShellRoute<SleepTipPage, SleepTipPageModel>("sleepTip");

            // Steps-related pages
            builder.Services.AddSingletonWithShellRoute<StepsMainPage, StepsMainPageModel>("steps");

            // Water-related pages
            builder.Services.AddTransientWithShellRoute<WaterMainPage, WaterMainPageModel>("water");

            // Account-related pages
            builder.Services.AddTransientWithShellRoute<ChooseSignUpSignInPage, ChooseSignUpSignInPageModel>("choose");
            builder.Services.AddTransientWithShellRoute<ProfileMainPage, ProfileMainPageModel>("profile");
            builder.Services.AddTransientWithShellRoute<RegisterPage, RegisterPageModel>("register");
            builder.Services.AddTransientWithShellRoute<RegisterConfirmationPage, RegisterConfirmationPageModel>("registerConfirmation");
            builder.Services.AddTransientWithShellRoute<LoginPage, LoginPageModel>("login");

            // Other pages
            builder.Services.AddTransientWithShellRoute<MainPage, MainPageModel>("main");

            return builder.Build();
        }
    }
}