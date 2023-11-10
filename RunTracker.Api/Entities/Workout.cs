namespace RunTracker.Api.Entities;

public sealed class Workout
{
    public int WorkoutId { get; set; }
    public int UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Notes { get; set; } = string.Empty;

    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
}