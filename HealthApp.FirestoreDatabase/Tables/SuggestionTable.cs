using Firebase.Auth;
using Firebase.Database;
using Models;

namespace HealthApp.FirestoreDatabase.Tables
{
    public sealed class SuggestionTable(FirebaseClient firebaseClient, FirebaseAuthClient firebaseAuthClient) : Table<Suggestion>(firebaseClient, firebaseAuthClient)
    {
    }
}
