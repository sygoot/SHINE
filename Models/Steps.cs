namespace Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="StartTime"></param>
    /// <param name="StartZoneOffset"></param>
    /// <param name="EndTime"></param>
    /// <param name="EndZoneOffset"></param>
    /// <param name="Count">Range(from = 1, to = 1000000)</param>
    public sealed record Steps(DateTime StartTime, DateTimeOffset StartZoneOffset, DateTime EndTime, DateTimeOffset EndZoneOffset, int Count);
}
