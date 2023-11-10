namespace RunTracker.Api.PipelineBehaviors;

using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

public class LoggingPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> _logger;

    public LoggingPipelineBehaviour(ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            LogRequest(request);

            var response = await next();

            LogResponse(response, stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw;
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    private void LogRequest(TRequest request)
    {
        _logger.LogInformation("Handling {RequestType} at {Timestamp}", typeof(TRequest).FullName, DateTime.UtcNow);
        foreach (var property in GetProperties(request))
        {
            _logger.LogInformation("{Property} : {@Value}", property.Name, property.GetValue(request, null));
        }
    }

    private void LogResponse(TResponse response, long elapsedMilliseconds)
    {
        _logger.LogInformation("Handled {ResponseType} at {Timestamp} (Elapsed: {Elapsed}ms)", typeof(TResponse).FullName, DateTime.UtcNow, elapsedMilliseconds);
    }

    private void LogError(Exception ex)
    {
        _logger.LogError(ex, "An error occurred at {Timestamp}", DateTime.UtcNow);
    }

    private IEnumerable<PropertyInfo> GetProperties(TRequest request)
    {
        return request.GetType().GetProperties();
    }
}
