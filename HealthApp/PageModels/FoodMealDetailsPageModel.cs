using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models;
using Models.Services.Database;

namespace HealthApp.PageModels
{
    [QueryProperty(nameof(CurrentMeal), "CurrentMeal")]
    public partial class FoodMealDetailsPageModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;

        public FoodMealDetailsPageModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            Ingredients = new ObservableCollection<Ingredient>();
        }

        [ObservableProperty]
        private Meal _currentMeal;

        public ObservableCollection<Ingredient> Ingredients { get; private set; }

        [RelayCommand]
        private async Task SaveMealAsync()
        {
            try
            {
                if (CurrentMeal != null)
                {
                    foreach (var ingredient in CurrentMeal.Ingredients)
                    {
                        CurrentMeal = CurrentMeal with
                        {
                            Carbohydrate = CurrentMeal.Carbohydrate + ingredient.Carbohydrate,
                            Calories = CurrentMeal.Calories + ingredient.Calories,
                            Fat = CurrentMeal.Fat + ingredient.Fat,
                            Protein = CurrentMeal.Protein + ingredient.Protein
                        };
                    }
                    if (CurrentMeal.Id > 0)
                    {
                        await _databaseService.MealTable.Update(CurrentMeal);
                    }
                    else
                    {
                        await _databaseService.MealTable.Add(CurrentMeal);
                    }

                    await Shell.Current.DisplayAlert("Sukces", "Posiłek został zapisany", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Nie udało się zapisać: {ex.Message}", "OK");
            }
        }

        partial void OnCurrentMealChanged(Meal value)
        {
            if (value != null)
            {
                // Zapewniamy, że Ingredients nie jest null
                Ingredients.Clear();
                if (value.Ingredients != null)
                {
                    foreach (var ingredient in value.Ingredients)
                    {
                        Ingredients.Add(ingredient);
                    }
                }

                // Aktualizujemy sumaryczne makroskładniki
                OnPropertyChanged(nameof(TotalCalories));
                OnPropertyChanged(nameof(TotalProtein));
                OnPropertyChanged(nameof(TotalCarbs));
                OnPropertyChanged(nameof(TotalFat));
            }
        }

        public double TotalCalories => Ingredients.Sum(i => i.Calories);
        public double TotalProtein => Ingredients.Sum(i => i.Protein);
        public double TotalCarbs => Ingredients.Sum(i => i.Carbohydrate);
        public double TotalFat => Ingredients.Sum(i => i.Fat);
    }
}
