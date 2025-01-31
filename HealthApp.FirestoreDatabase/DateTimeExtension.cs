namespace HealthApp.FirestoreDatabase
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// Calculate milliseconds to year 2000. All calculations are done in UTC0.
        /// For example if you will use current time the returned value will be now - year2000.
        /// </summary>
        /// <param name="givenDateTime">Given dateTime - can be before or after yer 2000.</param>
        /// <returns>Returned value can be negative if given time is before year 2000.</returns>
        public static long MillisecondsFromYear2000(this DateTime givenDateTime)
        {
            var millennium = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var difference = givenDateTime.ToUniversalTime() - millennium;

            return Convert.ToInt64(difference.TotalMilliseconds);
        }
    }
}
