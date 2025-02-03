using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Models;
using Models.Services.Database.Tables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace HealthApp.FirestoreDatabase.Tables
{
    public sealed class TargetTable(FirebaseClient firebaseClient, FirebaseAuthClient firebaseAuthClient) : Table<Target>(firebaseClient, firebaseAuthClient), ITargetTable
    {
        public IObservable<Target?> FindTargetByDate(DateTime date)
        {
            var loginUser = firebaseAuthClient.User;

            return loginUser is null
                ? Observable.Throw<Target?>(new Exception("Not login exception."))
                : firebaseClient
                .Child(TABLE_NAME)
                .Child(loginUser.Uid)
                .OrderBy(nameof(Target.Date))
                .EqualTo(DateOnly.FromDateTime(date).ToString())
                .OnceAsync<Target>(DEFAULT_TIMEOUT)
                .ToObservable()
                .SelectMany(targets => targets)
                .Select(target => target.Object)
                .FirstOrDefaultAsync();
        }
    }
}
