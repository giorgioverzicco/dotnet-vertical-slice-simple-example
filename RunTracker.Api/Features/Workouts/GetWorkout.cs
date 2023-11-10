using Carter;
using Dapper;
using FluentValidation;
using MediatR;
using RunTracker.Api.Database;
using RunTracker.Api.Entities;

namespace RunTracker.Api.Features.Workouts;

public sealed class GetWorkout
{
    public sealed record Query(int WorkoutId) : IRequest<Response?>;

    public sealed record Response(
        int WorkoutId,
        int UserId,
        List<int> ActivityIds,
        DateTime StartTime,
        DateTime EndTime,
        string Notes);

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.WorkoutId).GreaterThan(0);
        }
    }

    public sealed class Handler : IRequestHandler<Query, Response?>
    {
        private readonly RunTrackerDapperContext _context;

        public Handler(RunTrackerDapperContext context)
        {
            _context = context;
        }

        public async Task<Response?> Handle(Query request, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var workout = (await connection
                .QueryAsync<Workout, Activity, Workout>(
                    """
                    SELECT
                        w.WorkoutId,
                        w.UserId,
                        w.StartTime,
                        w.EndTime,
                        w.Notes,
                        a.ActivityId
                    FROM Workouts w
                    LEFT JOIN Activities a
                        ON a.WorkoutId = w.WorkoutId
                    WHERE w.WorkoutId = @WorkoutId
                    """,
                    (workout, activity) =>
                    {
                        workout.Activities.Add(activity);
                        return workout;
                    }, request, splitOn: "ActivityId")
                .WaitAsync(cancellationToken))
                .SingleOrDefault();

            if (workout is null)
            {
                return null;
            }

            return new Response(
                workout.WorkoutId,
                workout.UserId,
                workout.Activities.Select(x => x.ActivityId).ToList(),
                workout.StartTime,
                workout.EndTime,
                workout.Notes);
        }
    }
}

public sealed class GetWorkoutEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/workouts/{workoutId:int}", async (
            int workoutId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var request = new GetWorkout.Query(workoutId);
            var response = await sender.Send(request, cancellationToken);
            return response is null
                ? Results.NotFound()
                : Results.Ok(response);
        });
    }
}