using Models.Services.Database.Tables;

namespace Models
{
    public sealed record Water(DateTime StartTime, DateTimeOffset StartZoneOffset, DateTime EndTime, DateTimeOffset EndZoneOffset, double Volume, bool DataSent = false, long? Id = null) : Entity(Id);
}
