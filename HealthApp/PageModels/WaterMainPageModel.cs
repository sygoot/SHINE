using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models;
using Models.Services.Database;

namespace HealthApp.PageModels
{
    public partial class WaterMainPageModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;

        [ObservableProperty]
        private double _todaysWaterIntake;

        [ObservableProperty]
        private double _dailyProgress; // Teraz jako osobna właściwość z powiadamianiem

        private const double DailyGoal = 3000.0;

        public WaterMainPageModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            AddWaterCommand = new RelayCommand<string>(async (volumeLabel) => await AddWater(volumeLabel));
            _ = RefreshDailyIntake();
        }

        public IRelayCommand<string> AddWaterCommand { get; }

        private async Task AddWater(string volumeLabel)
        {
            if (double.TryParse(volumeLabel, out double volume))
            {
                var currentTime = DateTime.Now;
                var waterRecord = new Water(
                    RecordTime: currentTime,
                    StartZoneOffset: DateTimeOffset.Now,
                    Volume: volume
                );

                await _databaseService.WaterTable.Add(waterRecord).FirstAsync();
                await RefreshDailyIntake();

                // Animacja progresu
                var targetProgress = TodaysWaterIntake / DailyGoal;
                await Task.Delay(50); // Opóźnienie dla płynności animacji
                DailyProgress = targetProgress;
            }
        }

        private async Task RefreshDailyIntake()
        {
            var allRecords = await _databaseService.WaterTable.GetAll().FirstAsync();

            var sum = allRecords
                .Where(r => r.RecordTime.Date == DateTime.Today)
                .Sum(r => r.Volume);

            TodaysWaterIntake = sum;
            DailyProgress = TodaysWaterIntake / DailyGoal; // Aktualizuj progres
        }
    }
}