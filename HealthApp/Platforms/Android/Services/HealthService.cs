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
        // Użyj tej samej wartości HEALTH_CONNECT_URL, co w FetchStepsData:
        private const string HEALTH_CONNECT_URL =
            $"market://details?id={PROVIDER_PACKAGE_NAME}&url=healthconnect%3A%2F%2Fonboarding";

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

                var stepsRecord = new StepsRecord(
                    startTime!,
                    ZoneOffset.OfHours(2),
                    endTime,
                    ZoneOffset.OfHours(1),
                    1,
                    new Metadata()
                );
                var distanceRecord = new DistanceRecord(
                    startTime!,
                    ZoneOffset.OfHours(2),
                    endTime,
                    ZoneOffset.OfHours(1),
                    Length.InvokeMeters(11),
                    new Metadata()
                );

                List<string> neededPermissions = new List<string>();
                neededPermissions.Add(
                    HealthPermission.GetReadPermission(
                        Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(stepsRecord.Class)!
                    )
                );
                neededPermissions.Add(
                    HealthPermission.GetReadPermission(
                        Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(distanceRecord.Class)!
                    )
                );

                if (Platform.CurrentActivity is not MainActivity activity)
                    throw new InvalidOperationException("Current activity is not MainActivity");

                var availabilityStatus = HealthConnectClient
                    .GetSdkStatus(Platform.CurrentActivity, PROVIDER_PACKAGE_NAME);

                if (availabilityStatus == HealthConnectClient.SdkUnavailable)
                {
                    _logger.LogError("Sdk unavailable!");
                }

                if (availabilityStatus == HealthConnectClient.SdkUnavailableProviderUpdateRequired)
                {
                    Platform.CurrentActivity.StartActivity(
                        new Android.Content.Intent(
                            Android.Content.Intent.ActionView,
                            Android.Net.Uri.Parse(HEALTH_CONNECT_URL)
                        )
                        .SetPackage("com.android.vending")
                        .PutExtra("overlay", true)
                        .PutExtra("callerId", Platform.CurrentActivity.PackageName)
                    );

                    throw new InvalidOperationException("Health connect provider update required");
                }

                if (OperatingSystem.IsAndroidVersionAtLeast(26)
                    && (availabilityStatus == HealthConnectClient.SdkAvailable))
                {
                    // 1) Zdefiniuj metryki
                    ICollection<AggregateMetric> metrics =
                        new List<AggregateMetric>
                        {
                            StepsRecord.CountTotal,
                            DistanceRecord.DistanceTotal
                        };

                    var dataOriginFilter = new List<DataOrigin>();

                    // 2) Stwórz AggregationRequest
                    var request = new AggregateGroupByDurationRequest(
                        metrics,
                        TimeRangeFilter.After(startTime),
                        Duration.OfDays(1),
                        dataOriginFilter
                    );

                    // 3) Klient + sprawdzenie uprawnień
                    var healthConnectClientHelper = new HealthConnectClientHelper(
                        HealthConnectClient.GetOrCreate(Android.App.Application.Context)
                    );

                    var grantedPermissions = await healthConnectClientHelper.GetGrantedPermissions();
                    var missingPermissions = neededPermissions.Except(grantedPermissions).ToList();

                    if (missingPermissions.Count > 0)
                    {
                        grantedPermissions = await SimplePermissionHelper
                            .Request(new HashSet(missingPermissions));
                    }

                    var allPermissionsGranted = neededPermissions
                        .All(permission => grantedPermissions.Contains(permission));

                    if (allPermissionsGranted)
                    {
                        // 4) Wykonaj aggregator
                        var result = await healthConnectClientHelper
                            .AggregateGroupByDuration(request);

                        // 5) Wyciągnij z wyniku
                        var stepCountTotal = result
                            .FirstOrDefault(x => x.Result.Contains(StepsRecord.CountTotal))
                            ?.Result.Get(StepsRecord.CountTotal)
                            .JavaCast<Java.Lang.Number>();

                        if (stepCountTotal != null)
                        {
                            var resultSteps = Convert.ToInt32(stepCountTotal);
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

        // --------------------------------------------------------
        // Metoda do snu (analogiczny "flow", ale używa ReadRecords):
        // --------------------------------------------------------
        public async Task<List<Models.Sleep>> FetchSleepData(DateTime startingDay, DateTime endingDay)
        {
            try
            {
                //    // Przykład: zaczynamy od godziny 20:00 poprzedniego dnia
                //    startingDay = startingDay.AddDays(-1).Date.AddHours(20);

                var startTime = DateTimeToInstant(startingDay);
                var endTime = DateTimeToInstant(endingDay);

                //    // 1) Tworzymy SleepSessionRecord, by wyciągnąć KClass
                var sleepRecord = new SleepSessionRecord(
                    startTime!,
                    ZoneOffset.OfHours(2),
                    endTime,
                    ZoneOffset.OfHours(2),
                    "Sleep session",
                    null,
                    new List<SleepSessionRecord.Stage>(),
                    new Metadata()
                );

                var sleepRecordKotlinClass = Kotlin.Jvm.Internal.Reflection
                    .GetOrCreateKotlinClass(sleepRecord.Class);

                //    // 2) Uprawnienia do odczytu SleepSessionRecord
                List<string> neededPermissions = new List<string>
                    {
                        HealthPermission.GetReadPermission(sleepRecordKotlinClass)
                    };

                //    // 3) Sprawdzamy dostępność HealthConnect
                if (Platform.CurrentActivity is not MainActivity activity)
                    throw new InvalidOperationException("Current activity is not MainActivity");

                var availabilityStatus = HealthConnectClient
                    .GetSdkStatus(Platform.CurrentActivity, PROVIDER_PACKAGE_NAME);

                if (availabilityStatus == HealthConnectClient.SdkUnavailable)
                {
                    _logger.LogError("Sdk unavailable!");
                }

                if (availabilityStatus == HealthConnectClient.SdkUnavailableProviderUpdateRequired)
                {
                    Platform.CurrentActivity.StartActivity(
                        new Android.Content.Intent(
                            Android.Content.Intent.ActionView,
                            Android.Net.Uri.Parse(HEALTH_CONNECT_URL)
                        )
                        .SetPackage("com.android.vending")
                        .PutExtra("overlay", true)
                        .PutExtra("callerId", Platform.CurrentActivity.PackageName)
                    );

                    throw new InvalidOperationException("Health connect provider update required");
                }

                //    // 4) Jeśli SDK jest dostępne, przechodzimy do odczytu
                if (OperatingSystem.IsAndroidVersionAtLeast(26) && (availabilityStatus == HealthConnectClient.SdkAvailable))
                {
                    //        // a) Helper do HealthConnect
                    var healthConnectClientHelper = new HealthConnectClientHelper(
                        HealthConnectClient.GetOrCreate(Android.App.Application.Context)
                    );
                    //        // b) Sprawdź uprawnienia
                    var grantedPermissions = await healthConnectClientHelper.GetGrantedPermissions();
                    var missingPermissions = neededPermissions.Except(grantedPermissions).ToList();

                    if (missingPermissions.Count > 0)
                    {
                        grantedPermissions = await SimplePermissionHelper.Request(
                            new HashSet(missingPermissions)
                        );
                    }

                    var allPermissionsGranted = neededPermissions
                        .All(permission => grantedPermissions.Contains(permission));

                    //        if (!allPermissionsGranted)
                    //        {
                    //            _logger.LogError(
                    //                "Not all permissions granted. Please check Health Connect settings."
                    //            );
                    //            return null;
                    //        }

                    //        // c) Tworzymy ReadRecordsRequest (bardzo podobnie jak aggregator,
                    //        //    ale tym razem chcemy CAŁE rekordy, żeby mieć startTime / endTime).
                    //        var dataOriginFilter = new List<DataOrigin>();
                    //        var readRequest = new ReadRecordsRequest(
                    //            sleepRecordKotlinClass,               // recordType
                    //            TimeRangeFilter.Between(startTime, endTime),
                    //            dataOriginFilter,
                    //            false,     // ascendingOrder
                    //            1000,      // pageSize
                    //            null       // pageToken
                    //        );

                    //        // d) Wykonaj odczyt – w Twoim Helperze powinna być metoda 
                    //        //    ReadRecordsAsync<T>(ReadRecordsRequest request).
                    //        //    Jeśli nie ma, musisz ją dodać (proxy do _healthConnectClient).
                    //        var readResponse = await healthConnectClientHelper

                    //        // e) Otrzymujesz listę rekordów SleepSessionRecord w tym przedziale.
                    //        //    Możesz wybrać pierwszą sesję, wszystkie itd.
                    //        var firstSleepSession = readResponse.Records.FirstOrDefault();
                    //        if (firstSleepSession != null)
                    //        {
                    //            // f) Mamy start/koniec snu
                    //            var dtStart = firstSleepSession.StartTime.ToDateTime();
                    //            var dtEnd = firstSleepSession.EndTime.ToDateTime();

                    //            // Zmiana strefy w razie potrzeby:
                    //            //   var offsetStart = firstSleepSession.StartZoneOffset?.ToTimeSpan() ?? TimeSpan.Zero;
                    //            //   var offsetEnd   = firstSleepSession.EndZoneOffset?.ToTimeSpan()   ?? TimeSpan.Zero;
                    if (!allPermissionsGranted)
                    {
                        _logger.LogError("Not all permissions granted. Please check Health Connect settings.");
                        return [];
                    }

                    //            // Czas trwania (jeśli chcesz):
                    //            var duration = dtEnd - dtStart;
                    var sleepData = await healthConnectClientHelper.ReadSleepSessions(sleepRecord, startTime, endTime);

                    //            // g) Zwracasz w swoim Models.Sleep
                    //            //    (zakładam konstruktor: Models.Sleep(DateTime start, ..., DateTime end, ..., bool sth))
                    //            return new Models.Sleep(
                    //                dtStart,
                    //                firstSleepSession.StartZoneOffset?.ToTimeSpan() ?? TimeSpan.Zero,
                    //                dtEnd,
                    //                firstSleepSession.EndZoneOffset?.ToTimeSpan() ?? TimeSpan.Zero,
                    //                false
                    //            );
                    //        }
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    _logger.LogError(ex, "Error in FetchSleepData");
                    //}
                    if (sleepData != null && sleepData.Count > 0)
                    {
                        return sleepData
                            .Select(singleSleep => new Models.Sleep(
                                InstantToDateTime(singleSleep.StartTime),
                                null,
                                InstantToDateTime(singleSleep.EndTime),
                                null,
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
