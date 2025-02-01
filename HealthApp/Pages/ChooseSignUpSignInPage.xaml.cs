namespace HealthApp.Pages;

public partial class ChooseSignUpSignInPage : ContentPage
{
    public ChooseSignUpSignInPage(ChooseSignUpSignInPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Dispatcher.Dispatch(async () => await ((ChooseSignUpSignInPageModel)BindingContext).AppearingAsync());
    }

    private async void NavigateToLogin(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("login");
    }

    private async void NavigateToRegister(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("register");
    }
}