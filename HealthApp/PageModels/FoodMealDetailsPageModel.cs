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
        }

        [ObservableProperty]
        private Meal _currentMeal;

        public ObservableCollection<Ingredient> Ingredients { get; private set; } = new();

        [RelayCommand]
        private async Task SaveMealAsync()
        {
            try
            {
                if (CurrentMeal != null)
                {
                    // Jeśli posiłek już istnieje w bazie (ma ID) - aktualizuj
                    if (CurrentMeal.Id > 0)
                    {
                        await _databaseService.MealTable.Update(CurrentMeal);
                    }
                    // Jeśli to nowy posiłek - dodaj do bazy
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
            if (value?.Ingredients != null)
            {
                Ingredients = new ObservableCollection<Ingredient>(value.Ingredients);
                OnPropertyChanged(nameof(Ingredients));
            }

            OnPropertyChanged(nameof(TotalCalories));
            OnPropertyChanged(nameof(TotalProtein));
            OnPropertyChanged(nameof(TotalCarbs));
            OnPropertyChanged(nameof(TotalFat));
        }

        public double TotalCalories => Ingredients.Sum(i => i.Calories);
        public double TotalProtein => Ingredients.Sum(i => i.Protein);
        public double TotalCarbs => Ingredients.Sum(i => i.Carbohydrate);
        public double TotalFat => Ingredients.Sum(i => i.Fat);
    }
}