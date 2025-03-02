namespace Devtools.Test.TestServer;

public class DatabaseFixture
{
    public DatabaseFixture()
    {
        DevtoolsTestServer.CreateDatabaseServer().Wait();
    }

}