using Carter;
using FluentValidation;
using MediatR;
using RunTracker.Api.Database;
using RunTracker.Api.Entities;

namespace RunTracker.Api.Features.Activities;

public sealed class CreateActivity
{
    public sealed record Command(
        int UserId,
        string ActivityType,
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
            RuleFor(x => x.ActivityType).IsEnumName(typeof(ActivityType), caseSensitive: false);
            RuleFor(x => x.DistanceInMeters).GreaterThan(0);
            RuleFor(x => x.Duration).GreaterThanOrEqualTo(TimeSpan.FromSeconds(30));
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
            var activityType = Enum.Parse<ActivityType>(request.ActivityType, ignoreCase: true);
            var activity = new Activity
            {
                UserId = request.UserId,
                Type = activityType,
                Distance = request.DistanceInMeters,
                Duration = request.Duration.ToString(),
                Date = request.Date.ToDateTime(TimeOnly.MinValue),
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