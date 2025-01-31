using Models.Services.Database.Tables;

namespace Models.Services.Database
{
    public interface IDatabaseService
    {
        public IUserTable UserTable { get; }
        public Table<Ingredient> IngredientTable { get; }
        public Table<Meal> MealTable { get; }
        public Table<Sleep> SleepTable { get; }
        public Table<Steps> StepsTable { get; }
        public Table<Suggestion> SuggestionTable { get; }
        public Table<Target> TargetTable { get; }
        public Table<Water> WaterTable { get; }
    }
}
