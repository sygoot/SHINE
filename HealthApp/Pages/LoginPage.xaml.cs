namespace HealthApp.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}