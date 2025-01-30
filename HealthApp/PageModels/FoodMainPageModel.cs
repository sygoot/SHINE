using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Models.Services.Database;

namespace HealthApp.PageModels
{
    public partial class FoodMainPageModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;

        [ObservableProperty] private int totalCalories;
        [ObservableProperty] private int totalProtein;
        [ObservableProperty] private int totalCarbs;
        [ObservableProperty] private int totalFat;

        public FoodMainPageModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task LoadDailySummaryAsync()
        {
            var now = DateTime.Now;
            var midnight = now.Date; // Północ dzisiejszego dnia

            var allMeals = await _databaseService.MealTable.GetAll().FirstAsync();

            var mealsOfToday = allMeals.Where(m => m.Time >= midnight && m.Time <= now);
            var sumCalories = 0d;
            var sumProtein = 0d;
            var sumCarbs = 0d;
            var sumFat = 0d;

            foreach (var meal in mealsOfToday)
            {
                sumCalories += meal.Calories;
                sumProtein += meal.Protein;
                sumCarbs += meal.Carbohydrate;
                sumFat += meal.Fat;
            }

            TotalCalories = (int)sumCalories;
            TotalProtein = (int)sumProtein;
            TotalCarbs = (int)sumCarbs;
            TotalFat = (int)sumFat;
        }

    }
}
