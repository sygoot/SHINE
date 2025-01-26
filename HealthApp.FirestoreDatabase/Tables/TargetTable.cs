using Firebase.Auth;
using Firebase.Database;
using Models;

namespace HealthApp.FirestoreDatabase.Tables
{
    public sealed class TargetTable(FirebaseClient firebaseClient, FirebaseAuthClient firebaseAuthClient) : Table<Target>(firebaseClient, firebaseAuthClient)
    {
    }
}
