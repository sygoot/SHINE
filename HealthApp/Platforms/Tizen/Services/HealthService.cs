
using System;

namespace HealthApp.Services
{
    public partial class HealthService : IHealthService
    {
        public Task<int> FetchStepsData(DateTime startingDay, DateTime endingDay) => throw new NotImplementedException();
    }
}
