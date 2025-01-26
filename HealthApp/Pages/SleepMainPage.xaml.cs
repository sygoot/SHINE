namespace HealthApp.Pages;

public partial class SleepMainPage : ContentPage
{
    public SleepMainPage(SleepMainPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
    private async void NavigateToAddData(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("sleepAddData");
    }
}
