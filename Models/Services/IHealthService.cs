namespace Models.Services
{
    public interface IHealthService
    {
        public Task<int> FetchStepsData(DateTime startingDay, DateTime endingDay);
        public Task<List<Models.Sleep>> FetchSleepData(DateTime startingDay, DateTime endingDay);
    }
}
