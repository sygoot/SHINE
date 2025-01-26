namespace HealthApp.Pages;

public partial class FoodPortionDetailsPage : ContentPage
{
    public FoodPortionDetailsPage(FoodPortionDetailsPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
    public async void NavigateToAddMeal(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("foodAddMeal"); // TODO: Implement adding ingredient to list
    }
}

