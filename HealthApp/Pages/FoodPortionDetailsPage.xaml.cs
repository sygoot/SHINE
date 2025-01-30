namespace HealthApp.Pages;

public partial class FoodPortionDetailsPage : ContentPage
{
    public FoodPortionDetailsPage(FoodPortionDetailsPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }

    // Już nie musimy korzystać z Clicked w XAML, 
    // bo mamy to podpięte do komendy AddIngredientCommand
    // Ale jeśli chcesz, możesz zostawić metodę:
    public async void NavigateToAddMeal(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("foodAddMeal");
    }
}
