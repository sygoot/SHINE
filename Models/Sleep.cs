using Models.Services.Database.Tables;

namespace Models;
public sealed record Sleep(DateTime StartTime, DateTimeOffset StartZoneOffset, DateTime EndTime, DateTimeOffset EndZoneOffset, bool IsSetManually = false, long? Id = null) : Entity(Id);