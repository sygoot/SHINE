using Models.Services.Database.Tables;

namespace Models.Services.Database
{
    public interface IDatabaseService
    {
        public IUserTable UserTable { get; }
        public Table<Ingredient> IngredientTable { get; }
    }
}
