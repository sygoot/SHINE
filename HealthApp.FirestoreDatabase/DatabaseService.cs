using Models;
using Models.Services.Database;
using Models.Services.Database.Tables;

namespace HealthApp.FirestoreDatabase
{
    public sealed class DatabaseService : IDatabaseService
    {
        public IUserTable UserTable { get; private init; }

        public Table<Ingredient> IngredientTable { get; private init; }
        public Table<Meal> MealTable { get; private init; }
        public Table<Sleep> SleepTable { get; private init; }
        public Table<Steps> StepsTable { get; private init; }
        public Table<Suggestion> SuggestionTable { get; private init; }
        public ITargetTable TargetTable { get; private init; }
        public Table<Water> WaterTable { get; private init; }

        public DatabaseService(
            IUserTable userTable,
            Table<Ingredient> ingredientTable,
            Table<Meal> mealTable,
            Table<Sleep> sleepTable,
            Table<Steps> stepsTable,
            Table<Suggestion> suggestionTable,
            ITargetTable targetTable,
            Table<Water> waterTable)
        {
            UserTable = userTable;
            IngredientTable = ingredientTable;
            MealTable = mealTable;
            SleepTable = sleepTable;
            StepsTable = stepsTable;
            SuggestionTable = suggestionTable;
            TargetTable = targetTable;
            WaterTable = waterTable;
        }
    }
}
