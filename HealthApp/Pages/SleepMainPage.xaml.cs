namespace HealthApp.Pages;

public partial class SleepMainPage : ContentPage
{
    public SleepMainPage()
    {
        InitializeComponent();
        BindingContext = new SleepMainPageModel();
    }
    private async void NavigateToAddData(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("sleepAddData");
    }
}