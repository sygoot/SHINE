namespace HealthApp.Pages;

public partial class StepsMainPage : ContentPage
{
    public StepsMainPage(StepsMainPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
