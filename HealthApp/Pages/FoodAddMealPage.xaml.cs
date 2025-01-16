namespace HealthApp.Pages;

public partial class FoodAddMealPage : ContentPage
{
    public FoodAddMealPage()
    {
        InitializeComponent();
        BindingContext = new FoodAddMealPageModel();
    }
    public async void NavigateToPortionDetails(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("foodPortionDetails");
    }
    public async void NavigateToMealDetails(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("foodMealDetails");
    }
}