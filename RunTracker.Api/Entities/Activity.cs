namespace RunTracker.Api.Entities;

public sealed class Activity
{
    public int ActivityId { get; set; }
    public int UserId { get; set; }
    public ActivityType Type { get; set; }
    public float Distance { get; set; }
    public DateOnly Date { get; set; }
    public TimeSpan Duration { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public enum ActivityType
{
    Run,
    Bike,
    Swim
}
