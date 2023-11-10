using Microsoft.Data.Sqlite;
using System.Data;

namespace RunTracker.Api.Database;

public sealed class RunTrackerDapperContext
{
    private readonly string _connectionString;

    public RunTrackerDapperContext(IConfiguration configuration)
    {
        _connectionString = configuration["Database:Sqlite"]
                            ?? throw new NullReferenceException("Database:Sqlite key does not exists in configuration");
    }

    public IDbConnection CreateConnection() =>
        new SqliteConnection(_connectionString);
}