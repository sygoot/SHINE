namespace HealthApp.Pages;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }

    private void FirstNameCompleted(object sender, EventArgs e)
    {
        LastNameEntry.Focus();
    }

    private void LastNameCompleted(object sender, EventArgs e)
    {
        UsernameEntry.Focus();
    }

    private void UsernameCompleted(object sender, EventArgs e)
    {
        EmailEntry.Focus();
    }

    private void EmailCompleted(object sender, EventArgs e)
    {
        PasswordEntry.Focus();
    }

    private void PasswordCompleted(object sender, EventArgs e)
    {
        ConfirmPasswordEntry.Focus();
    }

    private void ConfirmPasswordCompleted(object sender, EventArgs e)
    {
        GenderPicker.Focus();
    }

    private void GenderPickerCompleted(object sender, EventArgs e)
    {
        HeightEntry.Focus();
    }

    private void HeightCompleted(object sender, EventArgs e)
    {
        WeightEntry.Focus();
    }

    private void WeightCompleted(object sender, EventArgs e)
    {
        DatePicker.Focus();
    }
}
