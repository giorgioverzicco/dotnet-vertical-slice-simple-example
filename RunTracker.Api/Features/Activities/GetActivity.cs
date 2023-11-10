using Carter;
using Dapper;
using FluentValidation;
using MediatR;
using RunTracker.Api.Database;
using RunTracker.Api.Entities;

namespace RunTracker.Api.Features.Activities;

public sealed class GetActivity
{
    public sealed record Query(int ActivityId) : IRequest<Response?>;

    public sealed record Response(
        int ActivityId,
        int UserId,
        string ActivityType,
        float DistanceInMeters,
        string Date,
        string Duration,
        string Location,
        string Notes);

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.ActivityId).GreaterThan(0);
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
            var activity = await connection
                .QuerySingleOrDefaultAsync<Activity>(
                    """
                    SELECT
                        ActivityId,
                        UserId,
                        Type,
                        Distance,
                        Date,
                        Duration,
                        Location,
                        Notes
                    FROM Activities
                    WHERE ActivityId = @ActivityId
                    """,
                    new { request.ActivityId })
                .WaitAsync(cancellationToken);

            if (activity is null)
            {
                return null;
            }

            return new Response(
                activity.ActivityId,
                activity.UserId,
                activity.Type.ToString(),
                activity.Distance,
                activity.Date.ToString("yy-MM-dd"),
                activity.Duration,
                activity.Location,
                activity.Notes);
        }
    }
}

public sealed class GetActivityEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/activities/{activityId:int}", async (
            int activityId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var request = new GetActivity.Query(activityId);
            var response = await sender.Send(request, cancellationToken);
            return response is null
                ? Results.NotFound()
                : Results.Ok(response);
        });
    }
}