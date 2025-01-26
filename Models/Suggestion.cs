using Models.Services.Database.Tables;

namespace Models
{
    public sealed record Suggestion(string Name, string Description, DateTime GenerationDate, AspectType Aspect, bool IsRead = false, long? Id = null) : Entity(Id);
}
