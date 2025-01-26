
using Models;
using Models.Services;

namespace HealthApp.Services
{
    public partial class HealthService : IHealthService
    {
        public Task<int> FetchStepsData(DateTime startingDay, DateTime endingDay) => throw new NotImplementedException();
        public Task<Sleep> FetchSleepData(DateTime startingDay, DateTime endingDay) => throw new NotImplementedException();
    }
}
