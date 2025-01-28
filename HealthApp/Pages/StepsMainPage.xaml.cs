namespace HealthApp.Pages;

public partial class StepsMainPage : ContentPage
{
    private readonly StepsMainPageModel _viewModel;

    public StepsMainPage(StepsMainPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadStepsAsync();
    }
}
