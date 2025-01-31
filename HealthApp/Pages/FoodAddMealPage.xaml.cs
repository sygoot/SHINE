namespace HealthApp.Pages;

public partial class FoodAddMealPage : ContentPage
{
    public FoodAddMealPage(FoodAddMealPageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Odśwież licznik składników w posiłku (np. jeśli wróciliśmy z FoodPortionDetailsPage)
        if (BindingContext is FoodAddMealPageModel vm)
        {
            vm.RefreshIngredientCount();
        }
    }

    private async void FoodSearchBar_SearchButtonPressed(object sender, EventArgs e)
    {
        if (BindingContext is FoodAddMealPageModel vm)
        {
            await vm.PerformSearchAsync();
        }
    }

    // Przycisk u góry -> przejście do FoodMealDetailsPage
    public async void NavigateToMealDetails(object sender, EventArgs e)
    {
        if (BindingContext is FoodAddMealPageModel vm)
        {
            // Przekazujemy nasz aktualny Meal do strony z podsumowaniem
            await Shell.Current.GoToAsync(
                $"{nameof(FoodMealDetailsPage)}",
                true,
                new Dictionary<string, object>
                {
                    { "CurrentMeal", vm.CurrentMeal }
                });
        }
    }

    // Na razie nie używamy NavigateToPortionDetails (zostawiłem, jeśli byś chciał)
    public async void NavigateToPortionDetails(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("foodPortionDetails");
    }
}
