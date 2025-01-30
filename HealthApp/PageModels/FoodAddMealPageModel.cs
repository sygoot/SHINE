using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HealthApp.FatSecretAPI;
using Models;
using FatApi = HealthApp.FatSecretAPI.FatSecretAPI;

namespace HealthApp.PageModels
{
    public partial class FoodAddMealPageModel : ObservableObject
    {
        private readonly FatApi _fatSecretApi;

        // Tekst wpisany w polu wyszukiwania
        [ObservableProperty]
        private string _searchQuery;

        // Kolekcja wyników zwróconych przez FatSecret
        [ObservableProperty]
        private ObservableCollection<StandarizedFoodResponse> _searchResults = new();

        // Tutaj trzymamy aktualnie budowany Meal (posiłek)
        [ObservableProperty]
        private Meal currentMeal;

        // Właściwość do wyświetlania liczby składników w CurrentMeal
        public int IngredientCount => CurrentMeal?.Ingredients?.Count ?? 0;

        public FoodAddMealPageModel(FatApi fatSecretApi)
        {
            _fatSecretApi = fatSecretApi;

            // Inicjalizujemy nowy Meal
            currentMeal = new Meal("Posilek", DateTime.Now, DateTimeOffset.Now, new(), 0, 0, 0, 0);

        }

        partial void OnCurrentMealChanged(Meal value)
        {
            // Gdybyśmy chcieli reagować na podmianę Meal,
            // moglibyśmy np. wywołać OnPropertyChanged(nameof(IngredientCount)).
        }

        // Ale tak naprawdę, gdy dodajemy Ingredient wewnątrz CurrentMeal, 
        // to Meal jest tą samą referencją, więc OnCurrentMealChanged nie wywoła się.
        // Dlatego, gdy wracamy z FoodPortionDetailsPage, 
        // odświeżamy IngredientCount:
        public void RefreshIngredientCount()
        {
            OnPropertyChanged(nameof(IngredientCount));
        }

        // Komenda wyszukiwania
        [RelayCommand]
        public async Task PerformSearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
                return;

            var results = await _fatSecretApi.GetFood(SearchQuery);

            SearchResults.Clear();
            foreach (var item in results)
            {
                SearchResults.Add(item);
            }
        }

        // Komenda kliknięcia w produkt
        [RelayCommand]
        private async Task SelectFood(StandarizedFoodResponse selectedFood)
        {
            if (selectedFood == null)
                return;

            // Przechodzimy do FoodPortionDetailsPage, przekazując wybrany produkt oraz
            // aktualny Meal (posiłek), do którego dodamy ten produkt.
            await Shell.Current.GoToAsync(
                $"{nameof(FoodPortionDetailsPage)}",
                true,
                new Dictionary<string, object>
                {
                    { "SelectedFood", selectedFood },
                    { "CurrentMeal", CurrentMeal }
                });
        }
    }
}
