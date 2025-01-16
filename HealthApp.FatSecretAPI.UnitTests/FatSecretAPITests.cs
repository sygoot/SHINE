using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace HealthApp.FatSecretAPI.UnitTests;

public class FatSecretAPITests : BaseTest
{
    public FatSecretAPITests(Container container, ITestOutputHelper logger) : base(container, logger)
    {
    }

    [Fact]
    public void FatSecretAPI_ShouldBeInitializedCorrectly()
    {
        var fatSecretAPI = Container.Provider.GetRequiredService<FatSecretAPI>();
        Assert.NotNull(fatSecretAPI);
    }

    [Fact]
    public async Task GetFood_ShouldReturnValidResponseWhenFoodNameIsPassed()
    {
        var fatSecretAPI = Container.Provider.GetRequiredService<FatSecretAPI>();
        var food = "chicken";
        Logger.WriteLine($"Searching for food: {food}");
        var response = await fatSecretAPI.GetFood(food);
        Assert.NotNull(response);
    }
    [Fact]
    public async Task GetFood_ShouldReturnSingularServingWithDescription100g()
    {
        var fatSecretAPI = Container.Provider.GetRequiredService<FatSecretAPI>();
        var food = "chicken";
        var listOfFoods = await fatSecretAPI.GetFood(food);
        Assert.All(listOfFoods, foodItem => Assert.True(foodItem.Serving.ServingDescription == "100g"));
    }
}
