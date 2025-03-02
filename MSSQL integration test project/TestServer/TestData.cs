using Devtools.Data.Entities;

namespace Devtools.TestServer;

public class TestData
{
    public static ArticleData ArticleData()
    {
        return new ArticleData
        {
            Id = Guid.NewGuid(),
            ArticleNumber = Guid.NewGuid().ToString(),
        };
    }
}