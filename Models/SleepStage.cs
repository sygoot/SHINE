namespace Models
{
    public sealed record SleepStage(DateTime StartTime, DateTime EndTime, SleepType Type)
    {
        private object stageType;
        private object duration;

    }
}