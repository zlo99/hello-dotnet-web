using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Read PostgreSQL connection info from environment variables
string host = Environment.GetEnvironmentVariable("PGHOST") ?? "localhost";
string user = Environment.GetEnvironmentVariable("PGUSER") ?? "postgres";
string password = Environment.GetEnvironmentVariable("PGPASSWORD") ?? "";
string database = Environment.GetEnvironmentVariable("PGDATABASE") ?? "postgres";

string connString =
    $"Host={host};Username={user};Password={password};Database={database};Timeout=5;Command Timeout=5";

// HTTP endpoint
app.MapGet("/", async () =>
{
    try
    {
        await using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand("SELECT NOW()", conn);
        var now = await cmd.ExecuteScalarAsync();

        return Results.Ok($"Hello World! PostgreSQL time is: {now}");
    }
    catch (Exception ex)
    {
        return Results.Problem(
            title: "Database connection failed",
            detail: ex.Message
        );
    }
});

// K8s-friendly
app.Run("http://0.0.0.0:8080");
