using Models.Services.Database;

namespace HealthApp.Pages;

public partial class FoodMainPage : ContentPage
{
    private readonly IDatabaseService _databaseService;

    public FoodMainPage(FoodMainPageModel model, IDatabaseService databaseService)
    {
        InitializeComponent();
        BindingContext = model;
        _databaseService = databaseService;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext is FoodMainPageModel model)
        {
            _ = LoadSummaryWhenReady(model);
        }
    }

    private async Task LoadSummaryWhenReady(FoodMainPageModel model)
    {
        await Task.Delay(100); // XDDDD
        await model.LoadDailySummaryAsync();
    }

    private async void NavigateToAddMeal(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("foodAddMeal");
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
    }
}
