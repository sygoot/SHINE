using Android.Runtime;
using AndroidX.Health.Connect.Client;
using AndroidX.Health.Connect.Client.Aggregate;

namespace HealthApp.Platforms.Android.Helpels
{
    internal class HealthConnectClientHelper(IHealthConnectClient healthConnectClient)
    {
        private readonly IHealthConnectClient healthConnectClient = healthConnectClient;

        private enum MappedCoroutineSingletonsStates
        {
            COROUTINE_SUSPENDED,
            UNDECIDED,
            RESUMED
        }

        public async Task<List<AggregationResultGroupedByDuration>> AggregateGroupByDuration(global::AndroidX.Health.Connect.Client.Request.AggregateGroupByDurationRequest request)
        {
            var taskCompletionSource = new TaskCompletionSource<Java.Lang.Object>();
            var result = healthConnectClient.AggregateGroupByDuration(request, new Continuation(taskCompletionSource, default));

            //redy to run coroutine task
            if (result is Java.Lang.Enum CoroutineSingletons)
            {
                var state = Enum.Parse<MappedCoroutineSingletonsStates>(CoroutineSingletons.ToString());
                if (state == MappedCoroutineSingletonsStates.COROUTINE_SUSPENDED)
                {
                    result = await taskCompletionSource.Task; //wait for the task to complete
                }
            }

            if (result is JavaList javaList)
            {
                //convert java list to .net list
                var resultList = new List<AggregationResultGroupedByDuration>();
                for (var i = 0; i < javaList.Size(); i++)
                {
                    if (javaList.Get(i) is AggregationResultGroupedByDuration item)
                    {
                        resultList.Add(item);
                    }
                }
                return resultList;
            }

            throw new Exception("Error while aggregating data");
        }


        public async Task<List<string>> GetGrantedPermissions()
        {
            var taskCompletionSource = new TaskCompletionSource<Java.Lang.Object>();
            var result = healthConnectClient.PermissionController.GetGrantedPermissions(new Continuation(taskCompletionSource, default));

            //ready to run coroutine task
            if (result is Java.Lang.Enum CoroutineSingletons)
            {
                var state = Enum.Parse<MappedCoroutineSingletonsStates>(CoroutineSingletons.ToString());
                if (state == MappedCoroutineSingletonsStates.COROUTINE_SUSPENDED)
                {
                    result = await taskCompletionSource.Task; //wait for the task to complete
                }
            }

            if (result is Kotlin.Collections.AbstractMutableSet abstractMutableSet)
            {
                var javaSet = abstractMutableSet.JavaCast<Java.Util.ISet>();
                return javaSet.ConvertISetToList();
            }

            throw new Exception("Error while getting granted permissions");
        }

    }
}
