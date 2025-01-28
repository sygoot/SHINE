using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace HealthApp.PageModels
{
    public partial class SleepMainPageModel : ObservableObject
    {
        private readonly HealthService _healthService;
        private readonly ILogger<SleepMainPageModel> _logger;

        [ObservableProperty]
        private DateTime _sleepStartTime;

        [ObservableProperty]
        private DateTime _sleepEndTime;

        [ObservableProperty]
        private TimeSpan _sleepDuration;

        public SleepMainPageModel(HealthService healthService, ILogger<SleepMainPageModel> logger)
        {
            _healthService = healthService;
            _logger = logger;
            FetchSleepCommand = new AsyncRelayCommand(LoadSleepAsync);
        }

        public IAsyncRelayCommand FetchSleepCommand { get; }

        public async Task LoadSleepAsync()
        {
            try
            {
                var startingDay = DateTime.Today;
                var endingDay = DateTime.Now;

                var sleepData = await _healthService.FetchSleepData(startingDay, endingDay);

                if (sleepData != null)
                {
                    SleepStartTime = sleepData.StartTime;
                    SleepEndTime = sleepData.EndTime;
                    SleepDuration = SleepEndTime - SleepStartTime;
                }
                else
                {
                    SleepStartTime = DateTime.MinValue;
                    SleepEndTime = DateTime.MinValue;
                    SleepDuration = TimeSpan.Zero;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when load sleep");
                SleepStartTime = DateTime.MinValue;
                SleepEndTime = DateTime.MinValue;
                SleepDuration = TimeSpan.Zero;
            }
        }
    }
}