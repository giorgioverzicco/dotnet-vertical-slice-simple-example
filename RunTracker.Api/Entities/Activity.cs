namespace RunTracker.Api.Entities;

public sealed class Activity
{
    public int ActivityId { get; set; }
    public int UserId { get; set; }
    public ActivityType Type { get; set; }
    public float Distance { get; set; }
    public DateTime Date { get; set; }
    public string Duration { get; set; } = "00:00:00";
    public string Location { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public enum ActivityType
{
    Run,
    Bike,
    Swim
}
