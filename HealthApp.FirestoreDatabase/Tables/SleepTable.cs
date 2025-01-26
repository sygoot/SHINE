using Firebase.Auth;
using Firebase.Database;
using Models;

namespace HealthApp.FirestoreDatabase.Tables
{
    public sealed class SleepTable(FirebaseClient firebaseClient, FirebaseAuthClient firebaseAuthClient) : Table<Sleep>(firebaseClient, firebaseAuthClient)
    {
    }
}
