using System.Reactive.Linq;  // ważne, aby móc wywołać .FirstAsync() na IObservable
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

        private const double DailyGoal = 3000.0;

        public double DailyProgress => TodaysWaterIntake / DailyGoal;

        public WaterMainPageModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            AddWaterCommand = new RelayCommand<string>(async (volumeLabel) => await AddWater(volumeLabel));

            // Pobierz aktualny stan przy tworzeniu
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

                // Dodaj rekord do bazy
                await _databaseService
                    .WaterTable
                    .Add(waterRecord)
                    .FirstAsync();  // .FirstAsync() “uruchamia” obserwowalny strumień i czeka na wynik

                // Odśwież sumę dzienną
                await RefreshDailyIntake();
            }
        }

        private async Task RefreshDailyIntake()
        {
            // Pobierz wszystkie rekordy jako IEnumerable<Water>
            var allRecords = await _databaseService
                .WaterTable
                .GetAll()
                .FirstAsync();  // czekamy, aż IObservable wyemituje wynik

            // W pamięci filtrujemy tylko dzisiejszą datę i sumujemy objętość
            var sum = allRecords
                .Where(r => r.RecordTime.Date == DateTime.Today)
                .Sum(r => r.Volume);

            TodaysWaterIntake = sum;
            OnPropertyChanged(nameof(DailyProgress));
        }
    }
}
