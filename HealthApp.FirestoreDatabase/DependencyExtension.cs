using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Database;
using Microsoft.Extensions.DependencyInjection;
using Models;
using Models.Services.Database;
using Models.Services.Database.Tables;

namespace HealthApp.FirestoreDatabase
{
    public static class DependencyExtension
    {
        public static void RegisterDatabaseServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(new FirebaseAuthClient(new()
            {
                ApiKey = "AIzaSyCFasMMK0TfNgqWmVz7Ap_rOCakjJDaZqY",
                AuthDomain = "shine-sygoot.firebaseapp.com",
                Providers = [new EmailProvider()],
                UserRepository = new FileUserRepository("UserRepo")
            }));

            serviceCollection.AddSingleton(serviceProvider => new FirebaseClient("https://shine-sygoot-default-rtdb.europe-west1.firebasedatabase.app/", new()
            {
                AuthTokenAsyncFactory = () => serviceProvider.GetRequiredService<FirebaseAuthClient>().User?.GetIdTokenAsync()
            }));

            serviceCollection.AddTransient<IUserTable, Tables.UserTable>();
            serviceCollection.AddTransient<Table<Ingredient>, Tables.IngredientTable>();
            serviceCollection.AddTransient<Table<Meal>, Tables.MealTable>();
            serviceCollection.AddTransient<Table<Sleep>, Tables.SleepTable>();
            serviceCollection.AddTransient<Table<Steps>, Tables.StepsTable>();
            serviceCollection.AddTransient<Table<Suggestion>, Tables.SuggestionTable>();
            serviceCollection.AddTransient<ITargetTable, Tables.TargetTable>();
            serviceCollection.AddTransient<Table<Water>, Tables.WaterTable>();

            serviceCollection.AddTransient<IDatabaseService, DatabaseService>();
        }
    }
}
