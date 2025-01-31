using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models;
using Models.Services.Database;

namespace HealthApp.PageModels;

public partial class TargetSelectionPageModel : ObservableObject
{
    private readonly IDatabaseService _databaseService;

    [ObservableProperty]
    private int? _stepsGoal;

    [ObservableProperty]
    private double? _hydrationGoalMl;

    [ObservableProperty]
    private double? _sleepGoal;

    [ObservableProperty]
    private int? _caloriesGoal; // Początkowo null

    [ObservableProperty]
    private CaloriesGoalType _caloriesGoalType = CaloriesGoalType.Maintenance; // Domyślnie Maintain

    [ObservableProperty]
    private string _caloriesGoalTypeString;

    [ObservableProperty]
    private bool _caloriesIsGenerated = false;

    public ObservableCollection<string> CaloriesGoalTypes { get; } =
        new ObservableCollection<string>(Enum.GetValues<CaloriesGoalType>()
            .Select(e => e.ToString()
                         .Replace("WeightLoss", "Weight Loss")
                         .Replace("WeightGain", "Weight Gain")
                         .Replace("Maintenance", "Maintenance")));

    public TargetSelectionPageModel(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
        LoadData();
    }

    private async void LoadData()
    {
        var latestTarget = await LoadLatestTarget(); // Pobierz ostatnie targety
        var user = await GetUserData();

        if (latestTarget != null)
        {
            StepsGoal = latestTarget.StepsGoal;
            HydrationGoalMl = latestTarget.HydrationGoalMl;
            SleepGoal = latestTarget.CaloriesGoal;
            CaloriesGoalType = latestTarget.CaloriesGoalType;
            CaloriesIsGenerated = latestTarget.CaloriesIsGenerated;

            CaloriesGoalTypeString = CaloriesGoalType.ToString()
                .Replace("WeightLoss", "Weight Loss")
                .Replace("WeightGain", "Weight Gain")
                .Replace("Maintenance", "Maintenance");

            if (CaloriesIsGenerated)
                CaloriesGoal = CalculateCaloriesBudget(user, CaloriesGoalType);
        }
    }

    /// <summary>
    /// Pobiera ostatnio zapisane targety użytkownika
    /// </summary>
    private async Task<Target?> LoadLatestTarget()
    {
        var targets = await _databaseService.TargetTable.GetAll().ToTask();
        return targets.OrderByDescending(t => t.Date).FirstOrDefault(); // Pobiera najnowszy rekord
    }

    private async Task<User?> GetUserData()
    {
        return await _databaseService.UserTable.Get().FirstOrDefaultAsync();
    }

    private int CalculateCaloriesBudget(User? user, CaloriesGoalType goalType)
    {
        if (user == null)
            return 2000;

        int age = DateTime.Today.Year - user.DateOfBirth.Year;
        if (user.DateOfBirth.Date > DateTime.Today.AddYears(-age))
            age--;

        double bmr = (10 * user.Weight) + (6.25 * user.Height) - (5 * age) + (user.Gender == Gender.Male ? 5 : -161);

        return goalType switch
        {
            CaloriesGoalType.Maintenance => (int)(bmr * 1.55),
            CaloriesGoalType.WeightLoss => (int)(bmr * 1.3),
            CaloriesGoalType.WeightGain => (int)(bmr * 1.7),
            _ => (int)(bmr * 1.55)
        };
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        try
        {
            var currentTime = DateTime.Now;

            var targetRecord = new Target(
                Date: currentTime,
                StepsGoal: StepsGoal ?? 10000,
                HydrationGoalMl: HydrationGoalMl ?? 2.0,
                CaloriesGoal: CaloriesGoal ?? 2500, // Jeśli użytkownik nie wpisze, ustaw domyślne
                CaloriesGoalType: CaloriesGoalType,
                CaloriesIsGenerated: CaloriesIsGenerated
            );

            await _databaseService.TargetTable.Add(targetRecord).FirstAsync();

            // Wyświetlenie toast
            await Shell.Current.DisplayAlert("Success", "Your targets were saved", "OK");

            // Przekierowanie na MainPage
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to save targets: {ex.Message}", "OK");
        }
    }

    partial void OnCaloriesGoalTypeChanged(CaloriesGoalType value)
    {
        CaloriesGoalTypeString = value.ToString()
            .Replace("WeightLoss", "Weight Loss")
            .Replace("WeightGain", "Weight Gain")
            .Replace("Maintenance", "Maintenance");

        if (CaloriesIsGenerated)
            GenerateCaloriesGoal();
    }

    partial void OnCaloriesGoalTypeStringChanged(string value)
    {
        CaloriesGoalType = value switch
        {
            "Weight Loss" => CaloriesGoalType.WeightLoss,
            "Weight Gain" => CaloriesGoalType.WeightGain,
            "Maintenance" => CaloriesGoalType.Maintenance,
            _ => CaloriesGoalType.Custom
        };
    }

    partial void OnCaloriesIsGeneratedChanged(bool value)
    {
        if (value)
            GenerateCaloriesGoal();
    }

    private async void GenerateCaloriesGoal()
    {
        var user = await GetUserData();
        if (user != null)
        {
            CaloriesGoal = CalculateCaloriesBudget(user, CaloriesGoalType);
        }
    }
}
