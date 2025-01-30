using Models.Services.Database.Tables;

namespace Models
{
    public sealed record Water(DateTime RecordTime, DateTimeOffset StartZoneOffset, double Volume, long? Id = null) : Entity(Id);
}
