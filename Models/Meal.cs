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
        // Domyślna inicjalizacja listy składników, aby zapobiec null
        public List<Ingredient> Ingredients { get; set; } = Ingredients ?? new List<Ingredient>();

        /// <summary>
        /// Aktualizuje wartości odżywcze na podstawie listy składników
        /// </summary>
        public Meal UpdateNutritionalValues()
        {
            return this with
            {
                Ingredients = Ingredients.ToList(), // Zapewniamy, że kopia listy jest przekazywana poprawnie
                Calories = Ingredients.Sum(i => i.Calories),
                Protein = Ingredients.Sum(i => i.Protein),
                Carbohydrate = Ingredients.Sum(i => i.Carbohydrate),
                Fat = Ingredients.Sum(i => i.Fat)
            };
        }
    }
}
