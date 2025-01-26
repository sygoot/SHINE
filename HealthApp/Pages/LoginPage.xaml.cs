namespace HealthApp.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }
    private async void NavigateToMain(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("main");
    }
}