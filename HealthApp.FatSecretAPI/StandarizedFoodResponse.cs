using FatSecretDotNet.ResponseObjects;

namespace HealthApp.FatSecretAPI;
public sealed class StandarizedFoodResponse
{
    public string Name { get; init; }
    public Serving Serving { get; init; }
    public int FoodId { get; init; }
}
