namespace HealthApp.Pages;

public partial class FoodMainPage : ContentPage
{
    public FoodMainPage()
    {
        InitializeComponent();
    }
    private async void NavigateToAddMeal(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("foodAddMeal");
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

    }
}