using Firebase.Auth;
using Firebase.Database;
using Models;

namespace HealthApp.FirestoreDatabase.Tables
{
    public sealed class StepsTable(FirebaseClient firebaseClient, FirebaseAuthClient firebaseAuthClient) : Table<Steps>(firebaseClient, firebaseAuthClient)
    {
    }
}
