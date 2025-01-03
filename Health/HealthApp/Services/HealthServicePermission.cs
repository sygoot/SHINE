using static Microsoft.Maui.ApplicationModel.Permissions;
#if ANDROID
using Android;
#endif

namespace HealthApp.Services
{
    public sealed class HealthServicePermission : BasePlatformPermission
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions
            => new (string androidPermission, bool isRuntime)[]
            {

            };
#endif
    }
}
