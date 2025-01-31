using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models;
using Models.Services.Database;

namespace HealthApp.PageModels;

public partial class ProfileMainPageModel : ObservableObject
{
    private readonly IDatabaseService _databaseService;

    [ObservableProperty]
    private string _selectedGender;

    [ObservableProperty]
    private double? _height;

    [ObservableProperty]
    private double? _weight;

    [ObservableProperty]
    private DateTime _dateOfBirth;

    public ObservableCollection<string> Genders { get; } =
        new ObservableCollection<string> { "Male", "Female" };

    public ProfileMainPageModel(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
        LoadData();
    }

    private async void LoadData()
    {
        var user = await GetUserData();

        if (user != null)
        {
            SelectedGender = user.Gender == Gender.Male ? "Male" : "Female";
            Height = user.Height;
            Weight = user.Weight;
            DateOfBirth = user.DateOfBirth;
        }
    }

    private async Task<User?> GetUserData()
    {
        return await _databaseService.UserTable.Get().FirstOrDefaultAsync();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        try
        {
            var genderEnum = SelectedGender == "Male" ? Gender.Male : Gender.Female;

            var userRecord = new User(
                FirstName: "", // Możesz dodać FirstName i inne pola, jeśli są potrzebne
                LastName: "",
                Username: "",
                Password: "",
                Email: "",
                Gender: genderEnum,
                Height: Height ?? 0,
                Weight: Weight ?? 0,
                DateOfBirth: DateOfBirth
            );

            await _databaseService.UserTable.Update(userRecord).FirstAsync();

            // Wyświetlenie Toast
            await Shell.Current.DisplayAlert("Success", "Your profile has been updated!", "OK");

            // Przekierowanie do poprzedniej strony
            await Shell.Current.GoToAsync("//Main");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to save profile: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}
