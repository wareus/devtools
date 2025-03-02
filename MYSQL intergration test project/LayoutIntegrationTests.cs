using System.Text.Json;
using Devtools.Data;
using Devtools.Test.TestServer;
using Microsoft.AspNetCore.SignalR.Client;

namespace Devtools.Test;

[Collection("Database collection")]
public class LayoutIntegrationTests
{
    [Fact]
    public async Task ShouldBeAbleToGetLayout()
    {
        var testServer = new DevtoolsTestServer(nameof(ShouldBeAbleToGetLayout));

        var layoutInDb = TestData.Layout();
        layoutInDb.Name = "name";

        testServer.DbContext.Add(layoutInDb);
        await testServer.DbContext.SaveChangesAsync();

        var response = await testServer.HttpClient.GetAsync("/layout");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var layout = JsonSerializer.Deserialize<TestEnvelope<Layout[]>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.Equal(layoutInDb.Name, layout!.Response.First().Name);
    }

    [Fact]
    public async Task ShouldBeAbleToTriggerReload()
    {
        var testServer = new DevtoolsTestServer(nameof(ShouldBeAbleToTriggerReload));

        await testServer.HttpClient.PostAsync("/layout/TriggerReload", new StringContent(string.Empty));

        var connection = new HubConnectionBuilder()
            .WithUrl(testServer.Server.BaseAddress + "testhub")
            .Build();


        var hasBeenCalled = false;
        connection.On<string>("SendTriggerReload", msg =>
        {
            hasBeenCalled = true;
        });

        await connection.StartAsync();

        Assert.True(hasBeenCalled);
    }
}