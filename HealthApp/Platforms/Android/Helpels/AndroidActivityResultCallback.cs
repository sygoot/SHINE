using AndroidX.Activity.Result;
using JObject = Java.Lang.Object;

namespace HealthApp.Platforms.Android.Helpels
{
    public class AndroidActivityResultCallback(Action<JObject?> callback) : JObject, IActivityResultCallback
    {
        public AndroidActivityResultCallback(TaskCompletionSource<JObject?> tcs) : this(tcs.SetResult)
        { }

        public void OnActivityResult(JObject? p0) => callback.Invoke(p0);
    }
}
