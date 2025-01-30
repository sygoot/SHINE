using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Microsoft.Extensions.Logging;
using Models;
using Models.Services.Database;

namespace HealthApp.PageModels
{
    public partial class RegisterPageModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly FirebaseAuthClient _firebaseAuthClient;
        private readonly ILogger<RegisterPageModel> _logger;

        public RegisterPageModel(IDatabaseService databaseService, FirebaseAuthClient firebaseAuthClient, ILogger<RegisterPageModel> logger)
        {
            _logger = logger;
            _databaseService = databaseService;
            _firebaseAuthClient = firebaseAuthClient;
            Genders = Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList();
            DateOfBirth = new DateTime(2000, 1, 1);
        }

        [ObservableProperty] private string _firstName;
        [ObservableProperty] private string _lastName;
        [ObservableProperty] private string _username;
        [ObservableProperty] private string _password;
        [ObservableProperty] private string _confirmPassword;
        [ObservableProperty] private string _email;
        [ObservableProperty] private Gender _selectedGender;
        [ObservableProperty] private double _height;
        [ObservableProperty] private double _weight;
        [ObservableProperty] private DateTime _dateOfBirth;

        public List<Gender> Genders { get; }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (!ValidateFields())
                return;

            try
            {
                var firebaseUser = await _firebaseAuthClient.CreateUserWithEmailAndPasswordAsync(Email, Password);
                _logger.LogInformation("User created in Firebase: {Uid}", firebaseUser.User.Uid);

                var user = new Models.User(
                    FirstName: FirstName,
                    LastName: LastName,
                    Username: Username,
                    Password: Password,
                    Email: Email,
                    Gender: SelectedGender,
                    Height: Height,
                    Weight: Weight,
                    DateOfBirth: DateOfBirth
                );

                await _databaseService.UserTable.Add(user);
                _logger.LogInformation("User added to the database");

                _firebaseAuthClient.SignInWithEmailAndPasswordAsync(Email, Password).ToObservable().Subscribe(
                    async userCredentials =>
                    {
                        _logger.LogInformation("User logged in successfully");
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await Shell.Current.GoToAsync("main");
                        });
                    },
                    async error =>
                    {
                        _logger.LogError(error, "Error during automatic login");
                        await ShowToast("Error while logging in");
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                await ShowToast("Check your e-mail address.");
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || FirstName.Length < 2)
            {
                ShowToast("First name must be at least 2 characters long");
                return false;
            }

            if (string.IsNullOrWhiteSpace(LastName) || LastName.Length < 2)
            {
                ShowToast("Last name must be at least 2 characters long");
                return false;
            }

            if (DateOfBirth > DateTime.Now.AddYears(-13))
            {
                ShowToast("You must be at least 13 years old");
                return false;
            }

            if (Password != ConfirmPassword)
            {
                ShowToast("Passwords do not match");
                return false;
            }

            if (Height < 50 || Height > 250)
            {
                ShowToast("Height must be between 50 cm and 250 cm");
                return false;
            }

            if (Weight < 20 || Weight > 300)
            {
                ShowToast("Weight must be between 20 kg and 300 kg");
                return false;
            }

            return true;
        }

        private async Task ShowToast(string message)
        {
            await Toast.Make(message, ToastDuration.Long).Show();
        }
    }
}
