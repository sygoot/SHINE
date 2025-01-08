using Android.Runtime;
using AndroidX.Health.Connect.Client;
using AndroidX.Health.Connect.Client.Aggregate;
using AndroidX.Health.Connect.Client.Permission;
using AndroidX.Health.Connect.Client.Records;
using AndroidX.Health.Connect.Client.Records.Metadata;
using AndroidX.Health.Connect.Client.Request;
using AndroidX.Health.Connect.Client.Time;
using AndroidX.Health.Connect.Client.Units;
using HealthApp.Platforms.Android.Helpels;
using Java.Time;
using Java.Util;
using Kotlin.Jvm;
using Microsoft.Extensions.Logging;

namespace HealthApp.Services
{
    public partial class HealthService : IHealthService
    {
        private readonly ILogger<HealthService> _logger;
        private const string PROVIDER_PACKAGE_NAME = "com.samsung.android.shealth";
        private const string HEALTH_CONNECT_URL = $"market://details?id={PROVIDER_PACKAGE_NAME}&url=healthconnect%3A%2F%2Fonboarding";

        public HealthService(ILogger<HealthService> logger)
        {
            _logger = logger;
            //var sdkStatus = HealthConnectClient.GetSdkStatus(Android.App.Application.Context);
            //if (sdkStatus == HealthConnectClient.SdkUnavailable)
            //{
            //    logger.LogError("Health SDK is not available on this device.");
            //}
            //else if (sdkStatus == HealthConnectClient.SdkUnavailableProviderUpdateRequired)
            //{
            //    logger.LogError("Health SDK is not available on this device. Please update the provider.");
            //}
            //else if (sdkStatus == HealthConnectClient.SdkAvailable)
            //{
            //    logger.LogInformation("Health SDK is available on this device.");

            //    var healthConnectClient = HealthConnectClient.GetOrCreate(Android.App.Application.Context);

            //    ReadStepsByTimeRange(healthConnectClient, DateTime.Now.AddDays(-1), DateTime.Now);
            //}
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
        public Instant? DateTimeToInstant(DateTime date)
            => Instant.OfEpochSecond(((DateTimeOffset)date).ToUnixTimeSeconds());

        public async Task<string> ExampleAsync()
        {
            try
            {

                var startOfDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local);
                var startTime = DateTimeToInstant(startOfDay);
                var endTime = DateTimeToInstant(DateTime.Now);

                var stepsRecord = new StepsRecord(startTime!, ZoneOffset.OfHours(2), endTime, ZoneOffset.OfHours(1), 1, new Metadata());
                var distanceRecord = new DistanceRecord(startTime!, ZoneOffset.OfHours(2), endTime, ZoneOffset.OfHours(1), Length.InvokeMeters(11), new Metadata());

                List<string> neededPermissions = [];
                neededPermissions.Add(HealthPermission.GetReadPermission(Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(stepsRecord.Class)!));
                neededPermissions.Add(HealthPermission.GetReadPermission(Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(distanceRecord.Class)!));

                if (Platform.CurrentActivity is not MainActivity activity)
                    throw new InvalidOperationException("Current activity is not MainActivity");

                var availabilityStatus = HealthConnectClient.GetSdkStatus(Platform.CurrentActivity, PROVIDER_PACKAGE_NAME);

                if (availabilityStatus == HealthConnectClient.SdkUnavailable)
                {
                    _logger.LogError("Sdk unavailable!");
                }

                if (availabilityStatus == HealthConnectClient.SdkUnavailableProviderUpdateRequired)
                {
                    Platform.CurrentActivity.StartActivity(new Android.Content.Intent(Android.Content.Intent.ActionView, Android.Net.Uri.Parse(HEALTH_CONNECT_URL))
                        .SetPackage("com.android.vending")
                        .PutExtra("overlay", true)
                        .PutExtra("callerId", Platform.CurrentActivity.PackageName));

                    throw new InvalidOperationException("Health connect provider update required");
                }


                if (OperatingSystem.IsAndroidVersionAtLeast(26) && (availabilityStatus == HealthConnectClient.SdkAvailable))
                {

                    ICollection<AggregateMetric> metrics = [StepsRecord.CountTotal, DistanceRecord.DistanceTotal];

                    var dataOriginFilter = new List<DataOrigin>();

                    var request = new AggregateGroupByDurationRequest(metrics, TimeRangeFilter.After(startTime), Duration.OfDays(1), dataOriginFilter);



                    var healthConnectClientHelper = new HealthConnectClientHelper(HealthConnectClient.GetOrCreate(Android.App.Application.Context));

                    var grantedPermissions = await healthConnectClientHelper.GetGrantedPermissions();
                    var missingPermissions = neededPermissions.Except(grantedPermissions).ToList();

                    if (missingPermissions.Count > 0)
                    {
                        grantedPermissions = await SimplePermissionHelper.Request(new HashSet(neededPermissions));
                    }

                    var allPermissionsGranted = neededPermissions.All(permission => grantedPermissions.Contains(permission));
                    if (allPermissionsGranted)
                    {
                        var Result = await healthConnectClientHelper.AggregateGroupByDuration(request);
                        var StepCountTotal = Result.FirstOrDefault(x => x.Result.Contains(StepsRecord.CountTotal))?.Result.Get(StepsRecord.CountTotal).JavaCast<Java.Lang.Number>();
                        var DistanceTotal = Result.FirstOrDefault(x => x.Result.Contains(DistanceRecord.DistanceTotal))?.Result.Get(DistanceRecord.DistanceTotal).JavaCast<AndroidX.Health.Connect.Client.Units.Length>();


                        if (StepCountTotal != null)
                        {
                            var resultString = StepCountTotal.ToString() + " " + DistanceTotal?.Meters.ToString();
                            _logger.LogInformation($"Sample result: {resultString}");
                            SemanticScreenReader.Announce(resultString);
                            return resultString;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Example :/");
            }

            return "No data";
        }

    }
}
