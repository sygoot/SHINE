namespace HealthApp.Pages;

public partial class FoodMealDetailsPage : ContentPage
{
    public FoodMealDetailsPage(FoodMealDetailsPageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
