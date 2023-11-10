using Carter;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RunTracker.Api.Database;
using RunTracker.Api.Entities;

namespace RunTracker.Api.Features.Workouts;

public sealed class CreateWorkout
{
    public sealed record Command(
        int UserId,
        List<int> ActivityIds,
        DateTime StartTime,
        DateTime EndTime,
        string Notes) : IRequest<Response>;

    public sealed record Response(int WorkoutId);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ActivityIds).NotEmpty();
            RuleForEach(x => x.ActivityIds).NotEmpty();
            RuleFor(x => x.StartTime).NotEmpty();
            RuleFor(x => x.EndTime).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<Command, Response>
    {
        private readonly RunTrackerDbContext _context;

        public Handler(RunTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var workout = new Workout
            {
                UserId = request.UserId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Notes = request.Notes,
                Activities = await _context.Activities
                    .Where(x => request.ActivityIds.Contains(x.ActivityId))
                    .ToListAsync(cancellationToken)
            };

            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response(workout.WorkoutId);
        }
    }
}

public sealed class CreateWorkoutEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/workouts", async (
            CreateWorkout.Command request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(request, cancellationToken);
            return Results.Created($"/workouts/{response.WorkoutId}", response);
        });
    }
}