namespace HealthApp.Pages;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }
    private async void NavigateToRegisterConfirmation(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("registerConfirmation");
    }
}