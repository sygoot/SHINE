namespace Models
{
    public sealed record Meal(string Name, DateTime Time, DateTimeOffset ZoneOffset, List<Ingredient> Ingredients, double Calories, double Protein, double Carbohydrate, double Fat);
}
