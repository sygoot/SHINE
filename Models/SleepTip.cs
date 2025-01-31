namespace Models;

public class SleepTip
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ExtendedDescription { get; set; }
    public string? ResourceLink { get; set; } // Opcjonalny link do artykułu

    public SleepTip(int id, string title, string description, string extendedDescription, string? resourceLink = null)
    {
        Id = id;
        Title = title;
        Description = description;
        ExtendedDescription = extendedDescription;
        ResourceLink = resourceLink;
    }
}
