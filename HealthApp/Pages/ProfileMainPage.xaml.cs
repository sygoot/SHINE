namespace HealthApp.Pages;

public partial class ProfileMainPage : ContentPage
{
    public ProfileMainPage(ProfileMainPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}