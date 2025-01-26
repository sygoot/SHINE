using Android.Runtime;
using Java.Util;

namespace HealthApp.Platforms.Android.Helpels
{
    internal static class JavaSetExtensions
    {
        public static List<string> ConvertISetToList(this ISet javaSet)
        {
            var listOfStrings = new List<string>();
            var iterator = javaSet.Iterator();

            while (iterator.HasNext)
            {
                var element = iterator.Next();
                if (element is Java.Lang.String)
                {
                    var stringElement = element.JavaCast<Java.Lang.String>().ToString();
                    listOfStrings.Add(stringElement);
                }
            }

            return listOfStrings;
        }
    }
}
