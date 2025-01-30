using Models.Services.Database.Tables;

namespace Models
{
    public sealed record Meal(
        string Name,
        DateTime Time,
        DateTimeOffset ZoneOffset,
        List<Ingredient> Ingredients,
        double Calories,
        double Protein,
        double Carbohydrate,
        double Fat,
        long? Id = null
    ) : Entity(Id)
    {
        /// <summary>
        /// Aktualizuje wartości odżywcze na podstawie listy składników
        /// </summary>
        public Meal UpdateNutritionalValues()
        {
            return this with
            {
                Calories = Ingredients.Sum(i => i.Calories),
                Protein = Ingredients.Sum(i => i.Protein),
                Carbohydrate = Ingredients.Sum(i => i.Carbohydrate),
                Fat = Ingredients.Sum(i => i.Fat)
            };
        }
    }
}
