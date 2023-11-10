using Microsoft.EntityFrameworkCore;
using RunTracker.Api.Entities;

namespace RunTracker.Api.Database;

public sealed class RunTrackerDbContext : DbContext
{
    public RunTrackerDbContext(DbContextOptions<RunTrackerDbContext> options)
        : base(options) { }

    public DbSet<Activity> Activities { get; set; } = null!;
    public DbSet<Workout> Workouts { get; set; } = null!;
}