using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Microsoft.Extensions.Logging;

namespace HealthApp.PageModels
{
    public partial class ChooseSignUpSignInPageModel : ObservableObject
    {
        private readonly FirebaseAuthClient _firebaseAuthClient;
        private readonly ILogger _logger;
        public ChooseSignUpSignInPageModel(FirebaseAuthClient firebaseAuthClient, ILogger<ChooseSignUpSignInPageModel> logger)
        {
            _firebaseAuthClient = firebaseAuthClient;
            _logger = logger;
        }

        [RelayCommand]
        public void Appearing()
        {
            if (_firebaseAuthClient.User != null)
            {
                Shell.Current.GoToAsync("//Main");
            }
        }
    }
}
