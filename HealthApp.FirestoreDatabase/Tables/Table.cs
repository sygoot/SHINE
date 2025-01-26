
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Models.Services.Database.Tables;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace HealthApp.FirestoreDatabase.Tables
{
    public class Table<T>(FirebaseClient firebaseClient, FirebaseAuthClient firebaseAuthClient) : Models.Services.Database.Tables.Table<T> where T : Entity
    {
        protected static readonly string TABLE_NAME = typeof(T).Name;
        protected readonly FirebaseClient firebaseClient = firebaseClient;
        protected readonly FirebaseAuthClient firebaseAuthClient = firebaseAuthClient;

        public override IObservable<long> Add(T entity)
        {
            var loginUser = firebaseAuthClient.User;

            var entityWithNewId = entity.CloneWithId(DateTime.UtcNow.MillisecondsFromYear2000());

            return firebaseClient
                .Child(TABLE_NAME)
                .Child(loginUser.Uid)
                .Child(entityWithNewId.Id.ToString())
                .PutAsync(entityWithNewId)
                .ToObservable()
                .Select(_ => entityWithNewId.Id!.Value);
        }
        public override IObservable<Unit> Delete(T entity)
        {
            var loginUser = firebaseAuthClient.User;

            return firebaseClient
                .Child(TABLE_NAME)
                .Child(loginUser.Uid)
                .Child(entity.Id.ToString())
                .DeleteAsync()
                .ToObservable();
        }
        public override IObservable<Unit> Delete(long entityId)
        {
            var loginUser = firebaseAuthClient.User;

            return firebaseClient
                .Child(TABLE_NAME)
                .Child(loginUser.Uid)
                .Child(entityId.ToString())
                .DeleteAsync()
                .ToObservable();
        }

        public override IObservable<Unit> DeleteAll()
        {
            var loginUser = firebaseAuthClient.User;

            return firebaseClient
                .Child(TABLE_NAME)
                .Child(loginUser.Uid)
                .DeleteAsync()
                .ToObservable();
        }

        public override IObservable<IEnumerable<T>> GetAll()
        {
            var loginUser = firebaseAuthClient.User;

            return firebaseClient
                .Child(TABLE_NAME)
                .Child(loginUser.Uid)
                .OnceAsync<T>()
                .ToObservable()
                .Select(result => result.Select(element => element.Object).ToList());
        }
        public override IObservable<T> GetById(long entityId)
        {
            var loginUser = firebaseAuthClient.User;

            return firebaseClient
                .Child(TABLE_NAME)
                .Child(loginUser.Uid)
                .Child(entityId.ToString())
                .OnceSingleAsync<T>()
                .ToObservable();
        }

        public override IObservable<T> ListenForChanges(long entityId)
        {
            var loginUser = firebaseAuthClient.User;

            return firebaseClient
                .Child(TABLE_NAME)
                .Child(loginUser.Uid)
                .Child(entityId.ToString())
                .AsObservable<T>()
                .Select(firebaseEvent => firebaseEvent.Object);
        }

        public override IObservable<Unit> Update(T entity)
        {
            var loginUser = firebaseAuthClient.User;

            return firebaseClient
                .Child(TABLE_NAME)
                .Child(loginUser.Uid)
                .Child(entity.Id.ToString())
                .PutAsync(entity)
                .ToObservable();
        }
    }
}
