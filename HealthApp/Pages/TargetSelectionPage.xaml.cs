namespace HealthApp.Pages;

public partial class TargetSelectionPage : ContentPage
{
    public TargetSelectionPage(TargetSelectionPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
