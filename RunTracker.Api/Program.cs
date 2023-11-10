using Carter;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RunTracker.Api.Database;
using RunTracker.Api.Middleware;
using RunTracker.Api.PipelineBehaviors;

var builder = WebApplication.CreateBuilder(args);
{
    var config = builder.Configuration;

    builder.Services.AddDbContext<RunTrackerDbContext>(o =>
        o.UseSqlite(config["Database:Sqlite"]));

    builder.Services.AddMediatR(c =>
        {
            c.RegisterServicesFromAssemblyContaining<Program>();
        })
        .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
        .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehaviour<,>));

    builder.Services.AddCarter();

    builder.Services.AddValidatorsFromAssemblyContaining<Program>();
    ValidatorOptions.Global.LanguageManager.Enabled = false;
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        // For development purposes, we'll delete and recreate the database on startup.
        var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RunTrackerDbContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    app.UseMiddleware<ValidationMiddleware>();
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.MapCarter();

    app.Run();
}