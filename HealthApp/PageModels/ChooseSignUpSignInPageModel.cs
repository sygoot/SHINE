using CommunityToolkit.Mvvm.ComponentModel;
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

        public async Task AppearingAsync()
        {
            if (_firebaseAuthClient.User != null)
            {

                await Shell.Current.GoToAsync("//Main");
            }
        }
    }
}
