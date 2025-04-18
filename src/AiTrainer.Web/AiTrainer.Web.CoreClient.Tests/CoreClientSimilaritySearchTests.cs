using System.Net;
using System.Text;
using System.Text.Json;
using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.CoreClient.Clients.Concrete;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.TestBase;
using AutoFixture;
using Microsoft.Extensions.Logging.Abstractions;

namespace AiTrainer.Web.CoreClient.Tests;

public class CoreClientSimilaritySearchTests: CoreClientTestBase
{
    public CoreClientSimilaritySearchTests()
    {
        SetUpBasicHttpContext();
    }

    [Fact]
    public async Task CoreClientSimilaritySearch_Should_Build_Request_Correctly()
    {
        //Arrange
        var stringJson = JsonSerializer.Serialize(new {Text = Faker.Lorem.Paragraph()});
        await using var memStream = new MemoryStream(Encoding.UTF8.GetBytes(stringJson));
        
        
        var input = _fixture
            .Build<CoreSimilaritySearchInput>()
            .With(x => x.DocStore, await JsonDocument.ParseAsync(memStream))
            .With(x => x.FileInput, memStream.ToArray())
            .Create();
        
        var response = _fixture
            .Build<CoreSimilaritySearchResponse>()
            .With(x => x.Items, _fixture.CreateMany<SimilaritySearchResponseItem>().ToArray())
            .Create();
        var mockedApiResponse = new CoreResponse<CoreSimilaritySearchResponse> { Data = response };

        var httpClient = CreateDefaultCoreClientHttpClient(HttpStatusCode.OK,
            mockedApiResponse,
            $"{_aiTrainerCoreConfiguration.BaseEndpoint}/api/faissrouter/similaritysearch");
        var service = new CoreClientSimilaritySearch(
            new NullLogger<CoreClientSimilaritySearch>(),
            new TestOptionsSnapshot<AiTrainerCoreConfiguration>(_aiTrainerCoreConfiguration).Object,
            httpClient,
            _mockHttpContextAccessor.Object
        );

        //Act
        var result = await service.TryInvokeAsync(input);
        
        //Assert
        Assert.True(httpClient.WasExpectedUrlCalled());
        Assert.NotNull(result);
        for (int i = 0; i < mockedApiResponse.Data.Items.Count; i++)
        {
            var expectedItem = mockedApiResponse.Data.Items.ElementAt(i);
            var actualItem  = result.Items.ElementAt(i);
            
            Assert.Equal(expectedItem.Metadata, actualItem.Metadata);
            Assert.Equal(expectedItem.PageContent, actualItem.PageContent);
        }
    }
}