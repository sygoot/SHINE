namespace Models.Services
{
    public interface IHealthService
    {
        public Task<int> ExampleAsync();
        public Task<Dictionary<string, object>> FetchHealthDataAsync(string dataType, string timeRange = "today");

    }
}
