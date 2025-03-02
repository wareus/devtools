using Devtools.Data;
using Devtools.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Testcontainers.MySql;

namespace Devtools.Test.TestServer;

public class DevtoolsTestServer
{
    private static string? connectionString;
    public HttpClient HttpClient { get; }
    public DatabaseContext DbContext { get; }
    public Microsoft.AspNetCore.TestHost.TestServer Server { get; }

    public DevtoolsTestServer(string name)
    {
        var newConnectionString = CreateDatabase(name);

        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {

            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints => { endpoints.MapHub<LayoutHub>("/testhub"); });
            });
        });

        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", newConnectionString);
        HttpClient = factory.CreateClient();
        var scope = factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        Server = factory.Server;
    }

    public string CreateDatabase(string name)
    {
        using var sqlConnection = new MySqlConnection(connectionString);
        sqlConnection.Open();
        using var command = new MySqlCommand($"CREATE DATABASE {name.ToLower()}", sqlConnection);
        command.ExecuteNonQuery();

        var builder = new MySqlConnectionStringBuilder(connectionString!)
        {
            Database = name.ToLower()
        };
        return builder.ConnectionString;

    }
    public static async Task CreateDatabaseServer()
    {
        var builder = new MySqlBuilder();
        var container = builder.Build();
        await container.StartAsync();
        connectionString = new MySqlConnectionStringBuilder(container.GetConnectionString())
        {
            UserID = "root"
        }.ConnectionString;

    }
}