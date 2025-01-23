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

        public async Task<int> ExampleAsync()
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
        public async Task<Dictionary<string, object>> FetchHealthDataAsync(string dataType, string timeRange = "today")
        {
            try
            {
                // 1. Determine the time range
                DateTime startOfDay, endOfDay;

                if (timeRange.ToLower() == "today")
                {
                    startOfDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local);
                    endOfDay = DateTime.Now;
                }
                else if (timeRange.Contains("-")) // Format YYYY-MM-DD
                {
                    var dates = timeRange.Split('-');
                    startOfDay = DateTime.Parse(dates[0].Trim());
                    endOfDay = dates.Length > 1 ? DateTime.Parse(dates[1].Trim()).AddDays(1).AddMilliseconds(-1) : startOfDay.AddDays(1).AddMilliseconds(-1);
                }
                else
                {
                    throw new ArgumentException("Invalid time range format. Use 'today' or 'YYYY-MM-DD[-YYYY-MM-DD]'.");
                }

                var startTime = DateTimeToInstant(startOfDay);
                var endTime = DateTimeToInstant(endOfDay);

                // 2. Create metrics, permissions, and request setup
                List<string> neededPermissions = new List<string>();
                ICollection<AggregateMetric> metrics = new List<AggregateMetric>();
                var dataOriginFilter = new List<DataOrigin>(); // Can be parameterized later

                // 3. Handle the specified data type
                switch (dataType.ToLower())
                {
                    case "steps":
                        var stepsRecord = new StepsRecord(startTime!, ZoneOffset.OfHours(2), endTime, ZoneOffset.OfHours(1), 1, new Metadata());
                        metrics.Add(StepsRecord.CountTotal);
                        neededPermissions.Add(HealthPermission.GetReadPermission(Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(stepsRecord.Class)!));
                        break;

                    case "sleep":
                        var sleepRecord = new SleepSessionRecord(startTime!, ZoneOffset.OfHours(2), endTime, ZoneOffset.OfHours(1), "Sleep session", null, new List<SleepSessionRecord.Stage>(), new Metadata());
                        metrics.Add(SleepSessionRecord.SleepDurationTotal);
                        neededPermissions.Add(HealthPermission.GetReadPermission(Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(sleepRecord.Class)!));
                        break;

                    case "hydration":
                        var hydrationRecord = new HydrationRecord(startTime!, ZoneOffset.OfHours(2), endTime, ZoneOffset.OfHours(1), Volume.InvokeLiters(0), new Metadata());
                        metrics.Add(HydrationRecord.VolumeTotal);
                        neededPermissions.Add(HealthPermission.GetReadPermission(Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(hydrationRecord.Class)!));
                        break;

                    default:
                        throw new NotSupportedException($"Unsupported data type: {dataType}");
                }

                // 4. Check SDK availability
                if (OperatingSystem.IsAndroidVersionAtLeast(26) && HealthConnectClient.GetSdkStatus(Platform.CurrentActivity, PROVIDER_PACKAGE_NAME) == HealthConnectClient.SdkAvailable)
                {
                    var request = new AggregateGroupByDurationRequest(metrics, TimeRangeFilter.Between(startTime, endTime), Duration.OfDays(1), dataOriginFilter);
                    var healthConnectClientHelper = new HealthConnectClientHelper(HealthConnectClient.GetOrCreate(Android.App.Application.Context));

                    // 5. Request permissions
                    var grantedPermissions = await healthConnectClientHelper.GetGrantedPermissions();
                    var missingPermissions = neededPermissions.Except(grantedPermissions).ToList();
                    if (missingPermissions.Count > 0)
                    {
                        grantedPermissions = await SimplePermissionHelper.Request(new HashSet(missingPermissions));
                    }

                    // 6. Fetch the data
                    if (neededPermissions.All(permission => grantedPermissions.Contains(permission)))
                    {
                        var result = await healthConnectClientHelper.AggregateGroupByDuration(request);
                        var response = new Dictionary<string, object>();

                        switch (dataType.ToLower())
                        {
                            case "steps":
                                var stepCount = result.FirstOrDefault(x => x.Result.Contains(StepsRecord.CountTotal))?.Result.Get(StepsRecord.CountTotal).JavaCast<Java.Lang.Number>();
                                response.Add("steps", stepCount?.ToString() ?? "0");
                                break;

                            case "sleep":
                                var sleepDuration = result.FirstOrDefault(x => x.Result.Contains(SleepSessionRecord.SleepDurationTotal))?.Result.Get(SleepSessionRecord.SleepDurationTotal).JavaCast<Java.Lang.Number>();
                                response.Add("sleep", sleepDuration?.ToString() ?? "0");
                                break;

                            case "hydration":
                                var hydrationVolume = result.FirstOrDefault(x => x.Result.Contains(HydrationRecord.VolumeTotal))?.Result.Get(HydrationRecord.VolumeTotal).JavaCast<Java.Lang.Number>();
                                response.Add("hydration", hydrationVolume?.ToString() ?? "0");
                                break;
                        }

                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FetchHealthDataAsync");
            }

            return new Dictionary<string, object> { { "error", "No data available" } };
        }
        public async Task<TimeSpan> FetchSleepDurationAsync()
        {
            try
            {
                // 1. Ustal zakres czasowy (od 21:00 dnia poprzedniego do teraz)
                var now = DateTime.Now;
                var startOfNight = new DateTime(now.Year, now.Month, now.Day - 1, 21, 0, 0, DateTimeKind.Local); // Wczoraj, 21:00
                var endOfNight = now; // Aktualny czas

                var startTime = DateTimeToInstant(startOfNight);
                var endTime = DateTimeToInstant(endOfNight);

                // 2. Tworzenie SleepSessionRecord i wymaganych uprawnień
                var sleepRecord = new SleepSessionRecord(startTime!, ZoneOffset.OfHours(2), endTime, ZoneOffset.OfHours(1), "Sleep session", null, new List<SleepSessionRecord.Stage>(), new Metadata());
                var neededPermissions = new List<string>
        {
            HealthPermission.GetReadPermission(Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(sleepRecord.Class)!)
        };

                // 3. Sprawdź dostępność SDK
                if (Platform.CurrentActivity is not MainActivity activity)
                    throw new InvalidOperationException("Current activity is not MainActivity");

                var availabilityStatus = HealthConnectClient.GetSdkStatus(Platform.CurrentActivity, PROVIDER_PACKAGE_NAME);

                if (availabilityStatus == HealthConnectClient.SdkUnavailable)
                {
                    _logger.LogError("SDK unavailable!");
                    return TimeSpan.Zero;
                }

                if (availabilityStatus == HealthConnectClient.SdkUnavailableProviderUpdateRequired)
                {
                    Platform.CurrentActivity.StartActivity(new Android.Content.Intent(Android.Content.Intent.ActionView, Android.Net.Uri.Parse(HEALTH_CONNECT_URL))
                        .SetPackage("com.android.vending")
                        .PutExtra("overlay", true)
                        .PutExtra("callerId", Platform.CurrentActivity.PackageName));

                    throw new InvalidOperationException("Health Connect provider update required.");
                }

                // 4. Jeśli SDK jest dostępne, kontynuuj
                if (OperatingSystem.IsAndroidVersionAtLeast(26) && availabilityStatus == HealthConnectClient.SdkAvailable)
                {
                    var metrics = new List<AggregateMetric> { SleepSessionRecord.SleepDurationTotal };
                    var dataOriginFilter = new List<DataOrigin>();

                    var request = new AggregateGroupByDurationRequest(metrics, TimeRangeFilter.Between(startTime, endTime), Duration.OfDays(1), dataOriginFilter);

                    var healthConnectClientHelper = new HealthConnectClientHelper(HealthConnectClient.GetOrCreate(Android.App.Application.Context));

                    // 5. Sprawdź uprawnienia
                    var grantedPermissions = await healthConnectClientHelper.GetGrantedPermissions();
                    var missingPermissions = neededPermissions.Except(grantedPermissions).ToList();

                    if (missingPermissions.Count > 0)
                    {
                        grantedPermissions = await SimplePermissionHelper.Request(new HashSet(missingPermissions));
                    }

                    // 6. Pobierz dane
                    var allPermissionsGranted = neededPermissions.All(permission => grantedPermissions.Contains(permission));
                    if (allPermissionsGranted)
                    {
                        var result = await healthConnectClientHelper.AggregateGroupByDuration(request);
                        var sleepDuration = result.FirstOrDefault(x => x.Result.Contains(SleepSessionRecord.SleepDurationTotal))?.Result.Get(SleepSessionRecord.SleepDurationTotal).JavaCast<Java.Lang.Number>();

                        if (sleepDuration != null)
                        {
                            // Konwersja milisekund na TimeSpan
                            var durationInMilliseconds = sleepDuration.LongValue();
                            return TimeSpan.FromMilliseconds(durationInMilliseconds);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FetchSleepDurationAsync");
            }

            // Zwraca TimeSpan.Zero w przypadku błędu
            return TimeSpan.Zero;
        }

    }
}
