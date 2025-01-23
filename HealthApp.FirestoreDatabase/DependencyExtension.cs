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
                ApiKey = "AIzaSyB5AGkNP1rhp9OLYIL5LHDt-5yVyzzmtOw",
                AuthDomain = "health-app-sygoot.firebaseapp.com",
                Providers = [new EmailProvider()],
                UserRepository = new FileUserRepository("UserRepo")
            }));

            serviceCollection.AddSingleton(serviceProvider => new FirebaseClient("https://health-app-sygoot-default-rtdb.europe-west1.firebasedatabase.app/", new()
            {
                AuthTokenAsyncFactory = async () => await serviceProvider.GetRequiredService<FirebaseAuthClient>().User?.GetIdTokenAsync()
            }));

            serviceCollection.AddTransient<IUserTable, Tables.UserTable>();
            serviceCollection.AddTransient<Table<Ingredient>, Tables.IngredientTable>();
            serviceCollection.AddTransient<IDatabaseService, DatabaseService>();
        }
    }
}
