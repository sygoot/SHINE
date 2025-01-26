namespace HealthApp.Pages;

public partial class SleepAddDataPage : ContentPage
{
    public SleepAddDataPage()
    {
        InitializeComponent();
    }

    private async void OnDatePickerButtonClicked(object sender, EventArgs e)
    {
        // Utwórz instancję DatePicker
        DatePicker datePicker = new DatePicker
        {
            Date = DateTime.Now, // Domyślna data
            MinimumDate = DateTime.Now.AddYears(-1),
            MaximumDate = DateTime.Now.AddYears(1)
        };

        // Obsłuż kliknięcie i pokaż pop-up (tu tylko placeholder)
        await DisplayAlert("Date Picker", "Systemowy DatePicker nie jest jeszcze zaimplementowany.", "OK");

        // Przykład: Aktualizacja tekstu przycisku na podstawie wybranej daty
        if (sender is Button button)
        {
            button.Text = datePicker.Date.ToString("d"); // Format: dzień/miesiąc/rok
        }
    }
}
