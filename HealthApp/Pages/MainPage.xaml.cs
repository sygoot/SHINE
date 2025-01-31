namespace HealthApp.Pages;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }

    private async void NavigateToTargetSelection(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("targetSelection");
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

    private void OnPreviousDayClicked(object sender, EventArgs e)
    {
        (BindingContext as MainPageModel)?.ChangeDate(-1); // Przewiń do poprzedniego dnia
    }

    private void OnNextDayClicked(object sender, EventArgs e)
    {
        (BindingContext as MainPageModel)?.ChangeDate(1); // Przewiń do następnego dnia
    }
}