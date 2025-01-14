namespace HealthApp.Pages;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

    }

    private async void NavigateToSteps(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("steps");
    }
    private async void NavigateToWater(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("water");
    }
    private async void NavigateToSleep(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("sleep");
    }
    private async void NavigateToFood(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("food");
    }
}