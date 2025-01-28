namespace HealthApp.Pages;

public partial class WaterMainPage : ContentPage
{
    public WaterMainPage(WaterMainPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
