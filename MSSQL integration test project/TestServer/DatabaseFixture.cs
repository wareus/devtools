namespace Devtools.TestServer;

public class DatabaseFixture
{
    public DatabaseFixture()
    {
        DevtoolsTestServer.CreateDatabaseServer().Wait();
    }

}