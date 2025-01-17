namespace Models;
public sealed record Sleep(DateTime StartTime, DateTimeOffset StartZoneOffset, DateTime EndTime, DateTimeOffset EndZoneOffset, List<SleepStage> SleepStages, bool DataSent = false);