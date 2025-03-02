using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Devtools.Data;
using Testcontainers.MsSql;

namespace Devtools.TestServer;

public class DevtoolsTestServer
{
    private static string _connectionString;
    public HttpClient HttpClient { get; }
    public DevtoolsDbContext DbContext { get; }

    public DevtoolsTestServer(string name)
    {
        var newConnectionString = CreateDatabase(name);

        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {

            });
        });

        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
        Environment.SetEnvironmentVariable("ConnectionStrings__Devtools", newConnectionString);
        HttpClient = factory.CreateClient();
        var scope = factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<DevtoolsDbContext>();

    }

    public string CreateDatabase(string name)
    {
        using var sqlConnection = new SqlConnection(_connectionString);
        sqlConnection.Open();
        using var command = new SqlCommand($"CREATE DATABASE {name.ToLower()}", sqlConnection);
        command.ExecuteNonQuery();

        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionString);
        builder.InitialCatalog = name;

        return builder.ConnectionString;

    }
    public static async Task CreateDatabaseServer()
    {
        var builder = new MsSqlBuilder();
        var container = builder.Build();
        await container.StartAsync();
        _connectionString = container.GetConnectionString();

    }
}