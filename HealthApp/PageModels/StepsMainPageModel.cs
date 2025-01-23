using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HealthApp.PageModels
{
    public partial class StepsMainPageModel : ObservableObject
    {
        private readonly HealthService _healthService;

        [ObservableProperty]
        private int _steps;

        public StepsMainPageModel(HealthService healthService)
        {
            _healthService = healthService;
            FetchStepsCommand = new AsyncRelayCommand(LoadStepsAsync);
        }

        public IAsyncRelayCommand FetchStepsCommand { get; }

        public async Task LoadStepsAsync()
        {
            try
            {
                var result = await _healthService.ExampleAsync();

                if (result != 0)
                {
                    Steps = result;
                }
                else
                {
                    Steps = 0; // Default if no data is found
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Steps = 0; // Reset in case of error
            }
        }
    }
}
