using Models.Services.Database.Tables;

namespace Models
{
    /// <summary>
    /// Pojedynczy składnik posiłku
    /// </summary>
    public sealed record Ingredient(
        string Name,
        double Grams,
        double Calories,
        double Protein,
        double Carbohydrate,
        double Fat,
        long? Id = null
    ) : Entity(Id);
}
