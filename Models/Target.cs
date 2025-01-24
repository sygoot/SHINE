using Models.Services.Database.Tables;

namespace Models
{
    public sealed record Target(DateTime Date, int StepsGoal, double HydrationGoal, int CaloriesGoal, CaloriesGoalType CaloriesGoalType = CaloriesGoalType.Custom, bool CaloriesIsGenerated = false, long? Id = null) : Entity(Id);
}
