using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Activity.Result;
using AndroidX.Health.Connect.Client;
using HealthApp.Platforms.Android.Helpels;
using JObject = Java.Lang.Object;

namespace HealthApp
{
    [IntentFilter(["androidx.health.ACTION_SHOW_PERMISSIONS_RATIONALE"])]
    [IntentFilter(["android.intent.action.VIEW_PERMISSION_USAGE"], Categories = ["android.intent.category.HEALTH_PERMISSIONS"])]
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        private TaskCompletionSource<JObject?>? _permissionRequestCompletedSource;
        private ActivityResultLauncher? _permissionRequestLauncher;
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create launcher to request permissions
            _permissionRequestLauncher = RegisterForActivityResult(
                PermissionController.CreateRequestPermissionResultContract(),
                new AndroidActivityResultCallback(result =>
                {
                    _permissionRequestCompletedSource?.TrySetResult(result);
                    _permissionRequestCompletedSource = null;
                }));
        }

        public Task RequestPermission(Java.Lang.Object permission, TaskCompletionSource<JObject?> whenCompletedSource)
        {
            _permissionRequestCompletedSource?.TrySetResult(null);
            _permissionRequestCompletedSource = whenCompletedSource;
            _permissionRequestLauncher?.Launch(permission);
            return whenCompletedSource.Task;
        }
    }
}
