using FatSecretDotNet;
using FatSecretDotNet.RequestObjects;

namespace HealthApp.FatSecretAPI;
public sealed class FatSecretAPI
{
    private readonly FatSecretClient clientFatService;
    public FatSecretAPI(FatSecretClient fatSecretClient)
    {
        this.clientFatService = fatSecretClient;
    }

    public async Task<List<StandarizedFoodResponse>> GetFood(string food)
    {
        FoodsSearchRequest request = new FoodsSearchRequest()
        {
            SearchExpression = food,
            MaxResults = 10,
        };

        var fatServiceAPIResult = await clientFatService.FoodsSearchAsync(request);

        List<StandarizedFoodResponse> standarizedFoodResponses = new List<StandarizedFoodResponse>();

        foreach (var foodItem in fatServiceAPIResult.Foods.Food)
        {
            FoodGetRequest foodGetRequest = new FoodGetRequest()
            {
                FoodId = Convert.ToInt32(foodItem.FoodId)
            };
            var foodWithServings = await clientFatService.FoodGetAsync(foodGetRequest);
            foreach (var serving in foodWithServings.Food.Servings.Serving)
            {
                if (serving.ServingDescription.ToLower().Replace(" ", "") == "100g")
                {
                    serving.ServingDescription = "100g";
                    standarizedFoodResponses.Add(new StandarizedFoodResponse()
                    {
                        FoodId = foodGetRequest.FoodId,
                        Name = foodItem.FoodName,
                        Serving = serving
                    });
                    break;
                }
            }
        }
        return standarizedFoodResponses;
    }
}