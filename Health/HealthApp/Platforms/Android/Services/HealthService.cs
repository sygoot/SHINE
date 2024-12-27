using AndroidX.Health.Connect.Client;
using AndroidX.Health.Connect.Client.Records;
using AndroidX.Health.Connect.Client.Time;
using Kotlin.Jvm;
using Microsoft.Extensions.Logging;

namespace HealthApp.Services
{
    public partial class HealthService : IHealthService
    {
        private readonly ILogger<HealthService> _logger;

        public HealthService(ILogger<HealthService> logger)
        {
            _logger = logger;
            var sdkStatus = HealthConnectClient.GetSdkStatus(Android.App.Application.Context);
            if (sdkStatus == HealthConnectClient.SdkUnavailable)
            {
                logger.LogError("Health SDK is not available on this device.");
            }
            else if (sdkStatus == HealthConnectClient.SdkUnavailableProviderUpdateRequired)
            {
                logger.LogError("Health SDK is not available on this device. Please update the provider.");
            }
            else if (sdkStatus == HealthConnectClient.SdkAvailable)
            {
                logger.LogInformation("Health SDK is available on this device.");

                var healthConnectClient = HealthConnectClient.GetOrCreate(Android.App.Application.Context);

                ReadStepsByTimeRange(healthConnectClient, DateTime.Now.AddDays(-1), DateTime.Now);
            }
        }
        //public async Task ReadStepsByTimeRange(HealthConnectClient healthConnectClient, DateTime startTime, DateTime endTime)
        //{
        //    try
        //    {
        //        var startInstant = Java.Time.Instant.OfEpochMilli(new DateTimeOffset(startTime).ToUnixTimeMilliseconds());
        //        var endInstant = Java.Time.Instant.OfEpochMilli(new DateTimeOffset(endTime).ToUnixTimeMilliseconds());

        //        var request = new ReadRecordsRequest(
        //            Java.Lang.Class.FromType(typeof(StepsRecord)),
        //            TimeRangeFilter.Between(startInstant, endInstant),
        //            new List<string>(), // Empty list for no additional filters
        //            false, // Not enabling partial results
        //            10, // Limit to 10 records
        //            null // No page token
        //        );

        //        var response = await Task.Run(() => healthConnectClient.ReadRecords(request));

        //        var records = response.Records.JavaCast<IList<StepsRecord>>();

        //        foreach (var stepRecord in records)
        //        {
        //            // Process each step record
        //            _logger.LogInformation($"Steps: {stepRecord.Count}, Start: {stepRecord.StartTime}, End: {stepRecord.EndTime}");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "Error reading step records.");
        //    }
        //}
        public async Task ReadStepsByTimeRange(IHealthConnectClient healthConnectClient, DateTime startTime, DateTime endTime)
        {
            try
            {
                var startInstant = Java.Time.Instant.OfEpochMilli(new DateTimeOffset(startTime).ToUnixTimeMilliseconds());
                var endInstant = Java.Time.Instant.OfEpochMilli(new DateTimeOffset(endTime).ToUnixTimeMilliseconds());

                var response = await Task.Run(() => healthConnectClient.ReadRecords(
                    new(
                        JvmClassMappingKt.GetKotlinClass(Java.Lang.Class.FromType(typeof(StepsRecord))),
                        TimeRangeFilter.Between(startInstant, endInstant),
                        [],
                        false,
                        10,
                        null
                    ),
                    null
                ));

                _logger.LogInformation($"Read {response} step records.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error reading step records.");
            }
        }
    }
}
