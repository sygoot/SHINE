using Firebase.Auth;
using Firebase.Database;
using Models;

namespace HealthApp.FirestoreDatabase.Tables
{
    public sealed class WaterTable(FirebaseClient firebaseClient, FirebaseAuthClient firebaseAuthClient) : Table<Water>(firebaseClient, firebaseAuthClient)
    {
    }
}
