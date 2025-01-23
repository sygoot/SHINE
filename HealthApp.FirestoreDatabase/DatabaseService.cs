using Models;
using Models.Services.Database;
using Models.Services.Database.Tables;

namespace HealthApp.FirestoreDatabase
{
    public sealed class DatabaseService : IDatabaseService
    {
        public IUserTable UserTable { get; private init; }

        public Table<Ingredient> IngredientTable { get; private init; }

        public DatabaseService(IUserTable userTable, Table<Ingredient> ingredientTable)
        {
            UserTable = userTable;
            IngredientTable = ingredientTable;
        }
    }
}
