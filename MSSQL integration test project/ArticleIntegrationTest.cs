
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Devtools.Api.Dto;
using Devtools.Data.Entities;
using Devtools.TestServer;

namespace Devtools;

[Collection("Database collection")]
public class ArticleIntegrationTest
{
    [Fact]
    public async Task InitializeArticles()
    {
        var testServer = new DevtoolsTestServer(nameof(InitializeArticles));

        using var multipartContent = new MultipartFormDataContent();

        var stream = new StreamContent(File.Open("test-articles.xlsx", FileMode.Open));

        stream.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        stream.Headers.Add("Content-Disposition", "form-data; name=\"import\"; filename=\"test-articles2.xlsx\"");
        multipartContent.Add(stream, "import", "test-articles2.xlsx");

        var response = await testServer.HttpClient.PostAsync("/api/articles/initialize", multipartContent);
        response.EnsureSuccessStatusCode();

        var articlesInDb = await testServer.DbContext.Articles.ToArrayAsync();

        Assert.Equal(2, articlesInDb!.Length);

    }

    [Fact]
    public async Task GetAllArticles()
    {
        var testServer = new DevtoolsTestServer(nameof(GetAllArticles));

        testServer.DbContext.Articles.Add(TestData.ArticleData());
        testServer.DbContext.Articles.Add(TestData.ArticleData());

        await testServer.DbContext.SaveChangesAsync();

        var response = await testServer.HttpClient.GetAsync("/api/articles");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var articles = JsonConvert.DeserializeObject<ArticleDto[]>(result);

        Assert.Equal(2, articles!.Length);

    }
}