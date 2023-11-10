using Carter;
using FluentValidation;
using MediatR;
using RunTracker.Api.Database;
using RunTracker.Api.Entities;

namespace RunTracker.Api.Features.Activities;

// This class is internal because must be only used by the CreateActivityEndpoint class,
// also sealed because it's not meant to be inherited from.
public sealed class CreateActivity
{
    public sealed record Command(
        int UserId,
        ActivityType ActivityType,
        float DistanceInMeters,
        TimeSpan Duration,
        DateOnly Date,
        string Location,
        string Notes) : IRequest<Response>;

    public sealed record Response(int ActivityId);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ActivityType).IsInEnum();
            RuleFor(x => x.DistanceInMeters).GreaterThan(0);
            RuleFor(x => x.Duration).GreaterThan(TimeSpan.FromSeconds(30));
            RuleFor(x => x.Date).NotEmpty();
            RuleFor(x => x.Location).NotEmpty();
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
            var activity = new Activity
            {
                UserId = request.UserId,
                Type = request.ActivityType,
                Distance = request.DistanceInMeters,
                Duration = request.Duration,
                Date = request.Date,
                Location = request.Location,
                Notes = request.Notes
            };

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response(activity.ActivityId);
        }
    }
}

public sealed class CreateActivityEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/activities", async (
            CreateActivity.Command request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(request, cancellationToken);
            return Results.Created($"/activities/{response.ActivityId}", response);
        });
    }
}