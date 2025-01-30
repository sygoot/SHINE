using System.Reactive.Linq; // Dodaj using dla modelu Steps
using Models;
using Models.Services.Database;

namespace HealthApp.Pages;

public partial class StepsMainPage : ContentPage
{
    private readonly StepsMainPageModel _viewModel;
    private readonly IDatabaseService _databaseService;

    public StepsMainPage(StepsMainPageModel viewModel, IDatabaseService databaseService)
    {
        _databaseService = databaseService;
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            // Wczytaj kroki
            var stepsCount = await _viewModel.LoadStepsAsync();

            // Stwórz nowy rekord kroków
            var stepsRecord = new Steps(
                StartTime: DateTime.Today, // Przykładowa data rozpoczęcia
                StartZoneOffset: DateTimeOffset.Now, // Przesunięcie strefy czasowej
                EndTime: DateTime.Now, // Przykładowa data zakończenia
                EndZoneOffset: DateTimeOffset.Now, // Przesunięcie strefy czasowej
                Count: stepsCount // Liczba kroków
            );

            // Zapisz rekord do bazy danych
            await _databaseService.StepsTable.Add(stepsRecord);

            // Powiadom użytkownika o sukcesie
            await Shell.Current.DisplayAlert("Sukces", "Kroki zostały zapisane", "OK");
        }
        catch (Exception ex)
        {
            // Powiadom użytkownika o błędzie
            await Shell.Current.DisplayAlert("Błąd", $"Nie udało się zapisać kroków: {ex.Message}", "OK");
        }
    }
}