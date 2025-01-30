using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HealthApp.FatSecretAPI;
using Models;

namespace HealthApp.PageModels
{
    [QueryProperty(nameof(SelectedFood), "SelectedFood")]
    [QueryProperty(nameof(CurrentMeal), "CurrentMeal")]
    public partial class FoodPortionDetailsPageModel : ObservableObject
    {
        // Przekazywane z poprzedniej strony
        [ObservableProperty]
        private StandarizedFoodResponse selectedFood;

        // Aktualnie budowany posiłek (ten sam, co w FoodAddMealPageModel)
        [ObservableProperty]
        private Meal currentMeal;

        // Domyślnie 100g
        [ObservableProperty]
        private double currentPortionInGrams = 100;

        private double ParseToDouble(string value)
        {
            return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result)
                ? result
                : 0;
        }

        // Obliczenia
        public double CalculatedCalories =>
            ParseToDouble(SelectedFood?.Serving?.Calories ?? "0") * (CurrentPortionInGrams / 100.0);

        public double CalculatedProtein =>
            ParseToDouble(SelectedFood?.Serving?.Protein ?? "0") * (CurrentPortionInGrams / 100.0);

        public double CalculatedCarbohydrate =>
            ParseToDouble(SelectedFood?.Serving?.Carbohydrate ?? "0") * (CurrentPortionInGrams / 100.0);

        public double CalculatedFat =>
            ParseToDouble(SelectedFood?.Serving?.Fat ?? "0") * (CurrentPortionInGrams / 100.0);

        partial void OnCurrentPortionInGramsChanged(double oldValue, double newValue)
        {
            OnPropertyChanged(nameof(CalculatedCalories));
            OnPropertyChanged(nameof(CalculatedProtein));
            OnPropertyChanged(nameof(CalculatedCarbohydrate));
            OnPropertyChanged(nameof(CalculatedFat));
        }

        // Komendy +/-
        [RelayCommand]
        private void IncreasePortion()
        {
            CurrentPortionInGrams += 10;
        }

        [RelayCommand]
        private void DecreasePortion()
        {
            if (CurrentPortionInGrams > 10)
                CurrentPortionInGrams -= 10;
        }

        [RelayCommand]
        private async Task AddIngredientAsync()
        {
            if (SelectedFood == null || CurrentMeal == null)
            {
                return;
            }

            var ingredient = new Ingredient(
                Name: SelectedFood.Name ?? "Produkt",
                Grams: CurrentPortionInGrams,
                Calories: CalculatedCalories,
                Protein: CalculatedProtein,
                Carbohydrate: CalculatedCarbohydrate,
                Fat: CalculatedFat
            );

            CurrentMeal.Ingredients.Add(ingredient);

            CurrentMeal = CurrentMeal.UpdateNutritionalValues();

            await Shell.Current.GoToAsync("..");
        }

    }
}
