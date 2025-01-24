using Models.Services.Database.Tables;

namespace Models;
public sealed record Sleep(DateTime StartTime, DateTimeOffset StartZoneOffset, DateTime EndTime, DateTimeOffset EndZoneOffset, List<SleepStage> SleepStages, bool DataSent = false, long? Id = null) : Entity(Id);