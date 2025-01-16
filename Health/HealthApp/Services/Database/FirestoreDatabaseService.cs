using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using HealthApp.Models;
using HealthApp.Services.Database.Tables;
using Microsoft.Extensions.Logging;

namespace HealthApp.Services.Database
{
    public class FirestoreDatabaseService : IDatabaseService
    {
        private readonly FirestoreDb firestoreDb;
        private readonly ILogger<FirestoreDatabaseService> logger;

        public UserTable UserTable => new();

        public FirestoreDatabaseService(ILogger<FirestoreDatabaseService> logger)
        {
            //var path = Path.Combine(FileSystem.AppDataDirectory, "google-services.json");
            //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            this.logger = logger;

            try
            {

                using (var resourceStream = FileSystem.OpenAppPackageFileAsync("google-services.json").Result)
                {
                    if (resourceStream is FileStream)
                    {
                        var absolutePath = (resourceStream as FileStream).Name;

                        //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", absolutePath);
                    }
                    var firestoreClient = new FirestoreClientBuilder()
                    {

                    };
                    firestoreDb = FirestoreDb.Create("health-app-sygoot");
                    firestoreDb = new FirestoreDbBuilder
                    {
                        //ProjectId = "health-app-sygoot",
                        //EmulatorDetection = EmulatorDetection.EmulatorOrProduction,
                        CredentialsPath = "google-services.json",
                        // JsonCredentials = File.ReadAllText(path),
                        //ApiKey = "AIzaSyCJvnY--9tfhhNvRW5J_xpfoxH1vh7DhY0",
                    }.Build();
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error initializing FirestoreDb");

            }
        }


        public async Task AddDataAsync(string collection, Dictionary<string, object> data)
        {
            var collectionRef = firestoreDb.Collection(collection);
            _ = await collectionRef.AddAsync(data);
        }


        public async Task AddUsers(List<User> users)
        {
            try
            {
                var collection = "users";
                var data = new Dictionary<string, object>()
            {
                { "Name", "Zegmint" },
                { "Age", 25 }
            };


                //users.ToDictionary(user => nameof(user.Name), user => (object)user.Name);
                var collectionRef = firestoreDb.Collection(collection);
                _ = await collectionRef.AddAsync(data);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding users to Firestore");
            }
        }

    }
}
