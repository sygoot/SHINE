using Android.App;
using Java.Util;
using JObject = Java.Lang.Object;


namespace HealthApp.Platforms.Android.Helpels
{
    internal class SimplePermissionHelper
    {
        private static readonly TimeSpan MaxPermissionRequestDuration = TimeSpan.FromMinutes(1);
        public static async Task<List<string>> Request(Java.Lang.Object permissions, CancellationToken cancellationToken = default)
        {
            try
            {

                if (Platform.CurrentActivity is not MainActivity activity)
                    throw new InvalidOperationException("Current activity is not MainActivity");

                var taskCompletionSource = new TaskCompletionSource<JObject?>();
                _ = Task.Delay(MaxPermissionRequestDuration, cancellationToken)
                    .ContinueWith(_ => taskCompletionSource.TrySetResult(null), TaskScheduler.Default);


                _ = new AlertDialog.Builder(activity)
                    .SetTitle("Health connect permission isn't granted")!
                    .SetMessage("Do you want to allow permissions?")!
                    .SetNegativeButton("Decline", (_, _) => taskCompletionSource.TrySetResult(null))!
                    .SetPositiveButton("Allow", (_, _) => RequestPermission())!
                    .Show();


                var result = await taskCompletionSource.Task.ConfigureAwait(false);

                if (result is null)
                    throw new InvalidOperationException("Permission request timed out");

                return ((ISet)result).ConvertISetToList();

                void RequestPermission() => activity.RequestPermission(permissions, taskCompletionSource);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }
    }
}
