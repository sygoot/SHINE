namespace Models
{
    public sealed record Suggestion(string Name, string Description, DateTime GenerationDate, AspectType Aspect, bool IsRead = false);
}
