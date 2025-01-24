using Models.Services.Database.Tables;

namespace Models
{
    public sealed record Ingredient(
        string Name,
        double Calcium,
        double Calories,
        double Carbohydrate,
        double Cholesterol,
        double Fat,
        double Fiber,
        double Iron,
        double MonounsaturatedFat,
        double PolyunsaturatedFat,
        double Potassium,
        double Protein,
        double SaturatedFat,
        double Sodium,
        double Sugar,
        double VitaminA,
        double VitaminC,
        long? Id = null) : Entity(Id);
}
