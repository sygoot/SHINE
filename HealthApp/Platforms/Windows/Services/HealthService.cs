
using Models.Services;

namespace HealthApp.Services
{
    public partial class HealthService : IHealthService
    {
        public Task<int> ExampleAsync() => throw new NotImplementedException();
        public Task<Dictionary<string, object>> FetchHealthDataAsync(string dataType, string timeRange = "today") => throw new NotImplementedException();
    }
}
