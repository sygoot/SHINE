using Firebase.Auth;
using Firebase.Database;
using Models;

namespace HealthApp.FirestoreDatabase.Tables
{
    public sealed class IngredientTable(FirebaseClient firebaseClient, FirebaseAuthClient firebaseAuthClient) : Table<Ingredient>(firebaseClient, firebaseAuthClient)
    {
    }
}
