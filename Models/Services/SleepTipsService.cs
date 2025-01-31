namespace HealthApp.Services;

using Models;

public class SleepTipsService
{
    private readonly List<SleepTip> _tips = new()
    {
        new SleepTip(1, "Regular Sleep Schedule.", "Go to bed and wake up at the same time every day.",
                     "Keeping a consistent sleep schedule helps regulate your body's internal clock and improves sleep quality.",
                     "https://www.sleepfoundation.org/sleep-hygiene"),
        new SleepTip(2, "Avoid Screens Before Bed", "Reduce screen time at least 1 hour before sleeping.",
                     "Blue light from screens interferes with melatonin production, making it harder to fall asleep.",
                     "https://www.healthline.com/nutrition/screen-time-and-sleep"),
        new SleepTip(3, "Create a Relaxing Bedtime Routine", "Engage in calming activities before bed, like reading or meditation.",
                     "A relaxing pre-sleep routine helps signal your body that it's time to wind down and rest.",
                     null)
    };

    public List<SleepTip> GetAllTips() => _tips;

    public SleepTip? GetTipById(int id) => _tips.FirstOrDefault(t => t.Id == id);
}
