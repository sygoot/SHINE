using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Models.Services.Database.Tables;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace HealthApp.FirestoreDatabase.Tables
{
    public sealed class UserTable(FirebaseClient firebaseClient, FirebaseAuthClient firebaseAuthClient) : IUserTable
    {
        private static readonly string TABLE_NAME = typeof(Models.User).Name;
        private readonly FirebaseClient firebaseClient = firebaseClient;
        private readonly FirebaseAuthClient firebaseAuthClient = firebaseAuthClient;
        public IObservable<Unit> Add(Models.User entity)
        {
            var loginUser = firebaseAuthClient.User;

            return firebaseClient
                .Child(TABLE_NAME)
                .Child(loginUser.Uid)
                .PutAsync(entity)
                .ToObservable();
        }

        public IObservable<Unit> Delete()
        {
            var loginUser = firebaseAuthClient.User;

            return firebaseClient
                .Child(TABLE_NAME)
                .Child(loginUser.Uid)
                .DeleteAsync()
                .ToObservable();
        }

        public IObservable<Models.User> Get()
        {
            var loginUser = firebaseAuthClient.User;

            return firebaseClient
                .Child(TABLE_NAME)
                .Child(loginUser.Uid)
                .OnceSingleAsync<Models.User>()
                .ToObservable();
        }

        public IObservable<Models.User> ListenForChanges()
        {
            var loginUser = firebaseAuthClient.User;

            return firebaseClient
                .Child(TABLE_NAME)
                .Child(loginUser.Uid)
                .AsObservable<Models.User>()
                .Select(firebaseEvent => firebaseEvent.Object);
        }

        public IObservable<Unit> Update(Models.User entity)
        {
            var loginUser = firebaseAuthClient.User;

            return firebaseClient
                .Child(TABLE_NAME)
                .Child(loginUser.Uid)
                .PutAsync(entity)
                .ToObservable();
        }
    }
}
