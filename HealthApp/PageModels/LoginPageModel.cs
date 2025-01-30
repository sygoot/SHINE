using System.Reactive.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Microsoft.Extensions.Logging;
using Models.Services.Database;

namespace HealthApp.PageModels;
public partial class LoginPageModel : ObservableObject
{
    private readonly IDatabaseService _databaseService;
    private readonly FirebaseAuthClient _firebaseAuthClient;
    private readonly ILogger<LoginPageModel> _logger;

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _password;

    public LoginPageModel(IDatabaseService databaseService, FirebaseAuthClient firebaseAuthClient, ILogger<LoginPageModel> logger)
    {
        _firebaseAuthClient = firebaseAuthClient;
        _databaseService = databaseService;
        _logger = logger;
    }

    [RelayCommand]
    public void Login()
    {
        if (!string.IsNullOrWhiteSpace(Username) || !string.IsNullOrWhiteSpace(Password))
        {
            _firebaseAuthClient.SignInWithEmailAndPasswordAsync(Username, Password).ToObservable().Subscribe(
               async userCredentials =>
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Shell.Current.GoToAsync("//Main");
                        _logger.LogInformation("User logged in");
                    });

                },
                async error =>
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                   {
                       Toast.Make("Error while logging in", ToastDuration.Long).Show();
                   });
                });
        }
    }
}
