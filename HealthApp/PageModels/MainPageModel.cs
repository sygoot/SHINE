using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Models;
using Models.Services.Database;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace HealthApp.PageModels
{
    public record ProgressData(double Value);
    public partial class MainPageModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<MainPageModel> _logger;
        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Today;

        [ObservableProperty]
        private bool _isNextDayEnabled = false;

        [ObservableProperty]
        private List<ProgressData> targetsData = [];

        [ObservableProperty]
        private List<ProgressData> sleepsData = [];

        [ObservableProperty]
        private List<ProgressData> stepsData = [];

        [ObservableProperty]
        private List<ProgressData> nutritionsData = [];

        [ObservableProperty]
        private List<ProgressData> hydrationsData = [];

        private Target? currentDayTarget;
        private IDisposable? updateAllStatesForCurrentDateDisposable;
        public MainPageModel(IDatabaseService databaseService, ILogger<MainPageModel> logger)
        {
            _databaseService = databaseService;
            _logger = logger;

            UpdateAllStatesForCurrentDate();
        }

        public void ChangeDate(int days)
        {
            SelectedDate = SelectedDate.AddDays(days);
            UpdateAllStatesForCurrentDate();
        }
        private void UpdateAllStatesForCurrentDate()
        {
            UpdateNextDayButtonState();

            updateAllStatesForCurrentDateDisposable?.Dispose();
            updateAllStatesForCurrentDateDisposable = UpdateTargets(SelectedDate)
                .SelectMany(_ => UpdateSleeps(SelectedDate))
                .SelectMany(_ => UpdateSteps(SelectedDate))
                .SelectMany(_ => UpdateNutritions(SelectedDate))
                .SelectMany(_ => UpdateHydrations(SelectedDate))
                .Subscribe(_ =>
                {
                    _logger.LogInformation("Updating statuses for current date finish with success.");
                },
                error =>
                {
                    _logger.LogError(error, "Updating statuses for current date fail.");
                },
                () =>
                {
                    _logger.LogInformation("Updating statuses for current date completed.");
                });
        }

        private void UpdateNextDayButtonState()
        {
            IsNextDayEnabled = SelectedDate < DateTime.Today;
        }
        private IObservable<Unit> UpdateTargets(DateTime currentDateTime)
        {
            return _databaseService.TargetTable.FindTargetByDate(currentDateTime)
                .Select(target =>
                {
                    currentDayTarget = target;

                    return Unit.Default;
                })
                .Do(_ => { }, error => _logger.LogWarning(error, "Database warning appear."))
                .Catch(Observable.Return(Unit.Default))
                .Select(_ =>
                {
                    TargetsData = [new(0)];
                    return Unit.Default;
                });
        }
        private IObservable<Unit> UpdateSleeps(DateTime currentDateTime)
        {
            return _databaseService.SleepTable.GetAll()
                .SubscribeOn(NewThreadScheduler.Default)
                .ObserveOn(NewThreadScheduler.Default)
                .SelectMany(sleeps => sleeps)
                .Where(sleep => sleep.StartTime >= currentDateTime.Date && sleep.StartTime <= currentDateTime.Date.AddDays(1))
                .Sum(sleep => (sleep.EndTime - sleep.StartTime).TotalSeconds)
                .Do(_ => { }, error => _logger.LogWarning(error, "Database warning appear."))
                .Catch(Observable.Return(0d))
                .Select(sleepTimeInSecondsForCurrentDate =>
                {
                    SleepsData = [new(sleepTimeInSecondsForCurrentDate)];

                    return Unit.Default;
                });

        }
        private IObservable<Unit> UpdateSteps(DateTime currentDateTime)
        {
            return _databaseService.StepsTable.GetAll()
                .SelectMany(steps => steps)
                .Where(steps => steps.StartTime >= currentDateTime.Date && steps.StartTime <= currentDateTime.Date.AddDays(1))
                .Sum(steps => steps.Count)
                .Do(_ => { }, error => _logger.LogWarning(error, "Database warning appear."))
                .Catch(Observable.Return(0))
                .Select(stepsCountForCurrentDate =>
                {
                    var stepsPercent = 0d;
                    if (currentDayTarget is not null)
                    {
                        stepsPercent = stepsCountForCurrentDate / currentDayTarget.StepsGoal;
                        stepsPercent = Math.Min(stepsPercent, 100d);
                    }

                    StepsData = [new(stepsPercent)];
                    TargetsData = [new(TargetsData[0].Value + stepsPercent / 3d)];

                    return Unit.Default;
                });
        }
        private IObservable<Unit> UpdateNutritions(DateTime currentDateTime)
        {
            return _databaseService.MealTable.GetAll()
                .SelectMany(meals => meals)
                .Where(meal => meal.Time >= currentDateTime.Date && meal.Time <= currentDateTime.Date.AddDays(1))
                .Sum(meal => meal.Calories)
                .Do(_ => { }, error => _logger.LogWarning(error, "Database warning appear."))
                .Catch(Observable.Return(0d))
                .Select(caloriesForCurrentDate =>
                {
                    var caloriesPercent = 0d;
                    if (currentDayTarget is not null)
                    {
                        caloriesPercent = caloriesForCurrentDate / currentDayTarget.CaloriesGoal;
                        caloriesPercent = Math.Min(caloriesPercent, 100d);
                    }

                    NutritionsData = [new(caloriesPercent)];
                    TargetsData = [new(TargetsData[0].Value + caloriesPercent / 3d)];

                    return Unit.Default;
                });

        }
        private IObservable<Unit> UpdateHydrations(DateTime currentDateTime)
        {
            return _databaseService.WaterTable.GetAll()
                .SelectMany(waters => waters)
                .Where(water => water.RecordTime >= currentDateTime.Date && water.RecordTime <= currentDateTime.Date.AddDays(1))
                .Sum(water => water.Volume)
                .Do(_ => { }, error => _logger.LogWarning(error, "Database warning appear."))
                .Catch(Observable.Return(0d))
                .Select(waterForCurrentDate =>
                {
                    var waterPercent = 0d;
                    if (currentDayTarget is not null)
                    {
                        waterPercent = waterForCurrentDate / currentDayTarget.HydrationGoalMl;
                        waterPercent = Math.Min(waterPercent, 100d);
                    }

                    HydrationsData = [new(waterPercent)];
                    TargetsData = [new(TargetsData[0].Value + waterPercent / 3d)];

                    return Unit.Default;
                });
        }
    }
}