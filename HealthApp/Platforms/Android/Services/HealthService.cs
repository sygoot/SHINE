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
using Microsoft.Extensions.Logging;
using Models.Services;

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
        }

        public Instant? DateTimeToInstant(DateTime date)
            => Instant.OfEpochSecond(((DateTimeOffset)date).ToUnixTimeSeconds());

        public static DateTime InstantToDateTime(Instant instant)
        {
            long epochMilli = instant.ToEpochMilli();
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(epochMilli);
            return dateTimeOffset.UtcDateTime;
        }

        public async Task<int> FetchStepsData(DateTime startingDay, DateTime endingDay)
        {
            try
            {

                var startOfDay = startingDay;
                var startTime = DateTimeToInstant(startOfDay);
                var endTime = DateTimeToInstant(endingDay);

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
                        //new Android.Health.Connect.DataTypes.SleepSessionRecord.Stage().Type
                        if (StepCountTotal != null)
                        {
                            var resultSteps = Convert.ToInt32(StepCountTotal);
                            return resultSteps;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Example :/");
            }

            return -1;
        }

        public async Task<List<Models.Sleep>> FetchSleepData(DateTime startingDay, DateTime endingDay)
        {
            try
            {
                startingDay = startingDay.AddDays(-1).Date.AddHours(20);
                var startTime = DateTimeToInstant(startingDay);
                var endTime = DateTimeToInstant(endingDay);

                var sleepRecord = new SleepSessionRecord(startTime!, ZoneOffset.OfHours(2), endTime, ZoneOffset.OfHours(1), "Sleep session", null, new List<SleepSessionRecord.Stage>(), new Metadata());

                List<string> neededPermissions = [];
                neededPermissions.Add(HealthPermission.GetReadPermission(Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(sleepRecord.Class)!));

                foreach (var permission in neededPermissions)
                {
                    _logger.LogInformation($"Needed permission: {permission}");
                }

                if (Platform.CurrentActivity is not MainActivity activity)
                    throw new InvalidOperationException("Current activity is not MainActivity");

                var availabilityStatus = HealthConnectClient.GetSdkStatus(Platform.CurrentActivity, "com.google.android.apps.healthdata");

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
                    var healthConnectClientHelper = new HealthConnectClientHelper(HealthConnectClient.GetOrCreate(Android.App.Application.Context));

                    var grantedPermissions = await healthConnectClientHelper.GetGrantedPermissions();

                    foreach (var permission in grantedPermissions)
                    {
                        _logger.LogInformation($"Granted permission: {permission}");
                    }

                    var missingPermissions = neededPermissions.Except(grantedPermissions).ToList();

                    foreach (var permission in missingPermissions)
                    {
                        _logger.LogWarning($"Missing permission: {permission}");
                    }

                    if (missingPermissions.Count > 0)
                    {
                        foreach (var permission in missingPermissions)
                        {
                            _logger.LogError($"Missing permission: {permission}");
                        }
                        grantedPermissions = await SimplePermissionHelper.Request(new HashSet(missingPermissions));
                    }

                    var allPermissionsGranted = neededPermissions.All(permission => grantedPermissions.Contains(permission));

                    if (!allPermissionsGranted)
                    {
                        _logger.LogError("Not all permissions granted. Please check Health Connect settings.");
                        return [];
                    }

                    var sleepData = await healthConnectClientHelper.ReadSleepSessions(sleepRecord, startTime, endTime);

                    if (sleepData != null && sleepData.Count > 0)
                    {
                        return sleepData
                            .Select(singleSleep => new Models.Sleep(
                                InstantToDateTime(singleSleep.StartTime),
                                Convert.ToDateTime(singleSleep.StartZoneOffset),
                                Convert.ToDateTime(singleSleep.EndTime),
                                Convert.ToDateTime(singleSleep.EndZoneOffset),
                                false
                            ))
                            .ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FetchSleepData");
            }

            return []; // Wartość błędu
        }

    }
}
