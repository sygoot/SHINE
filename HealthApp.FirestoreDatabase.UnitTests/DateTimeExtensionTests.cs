namespace HealthApp.FirestoreDatabase.UnitTests
{
    public sealed class DateTimeExtensionTests
    {
        [Fact]
        public void MillisecondsFromYear2000_Should_Return_444_When_Given_DateTime_Is_444_Second_After_Year_2000()
        {
            var millisecondsAfterYear2000 = 444d;
            var currentTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddMilliseconds(millisecondsAfterYear2000);

            var milliseconds = currentTime.MillisecondsFromYear2000();

            Assert.Equal(millisecondsAfterYear2000, milliseconds);
        }
        [Fact]
        public void MillisecondsFromYear2000_Should_Return_Negative_555_When_Given_DateTime_Is_555_Second_Before_Year_2000()
        {
            var millisecondsAfterYear2000 = -555d;
            var currentTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddMilliseconds(millisecondsAfterYear2000);

            var milliseconds = currentTime.MillisecondsFromYear2000();

            Assert.Equal(millisecondsAfterYear2000, milliseconds);
        }
        [Fact]
        public void MillisecondsFromYear2000_Should_Return_Negative_777_When_Given_DateTime_Is_777_Second_Before_Year_2000_And_Have_LocalTimeZone()
        {
            var millisecondsAfterYear2000 = -777d;
            var currentTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddMilliseconds(millisecondsAfterYear2000)
                .ToLocalTime();

            var milliseconds = currentTime.MillisecondsFromYear2000();

            Assert.Equal(millisecondsAfterYear2000, milliseconds);
        }
    }
}
