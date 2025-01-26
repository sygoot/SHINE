using Firebase.Auth;
using Firebase.Database;
using Models;

namespace HealthApp.FirestoreDatabase.Tables
{
    public sealed class MealTable(FirebaseClient firebaseClient, FirebaseAuthClient firebaseAuthClient) : Table<Meal>(firebaseClient, firebaseAuthClient)
    {
    }
}
