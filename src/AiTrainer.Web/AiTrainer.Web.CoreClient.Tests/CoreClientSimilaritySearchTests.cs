using System.Text;
using System.Text.Json;
using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.CoreClient.Clients.Concrete;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.TestBase;
using AutoFixture;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace AiTrainer.Web.CoreClient.Tests;

public class CoreClientSimilaritySearchTests: CoreClientTestBase
{
    private readonly Mock<ILogger<CoreClientSimilaritySearch>> _mockLogger = new();
    private readonly CoreClientSimilaritySearch _coreClientSimilaritySearch;

    public CoreClientSimilaritySearchTests()
    {
        SetUpBasicHttpContext();
        
        _httpTest.WithSettings(x =>
        {
            x.JsonSerializer = ApiConstants.DefaultCamelFlurlJsonSerializer;
        });
        
        _coreClientSimilaritySearch = new CoreClientSimilaritySearch(
            _mockLogger.Object,
            new TestOptionsSnapshot<AiTrainerCoreConfiguration>(_aiTrainerCoreConfiguration).Object,
            _mockHttpContextAccessor.Object,
            ApiConstants.DefaultCamelFlurlJsonSerializer
        );
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
            .Create();
        
        var response = _fixture
            .Build<SimilaritySearchCoreResponse>()
            .With(x => x.Items, _fixture.CreateMany<SimilaritySearchResponseItem>().ToArray())
            .Create();
        var mockedApiResponse = new CoreResponse<SimilaritySearchCoreResponse> { Data = response };
        
        _httpTest
            .ForCallsTo($"{_aiTrainerCoreConfiguration.BaseEndpoint}/api/faissrouter/similaritysearch")
            .WithVerb(HttpMethod.Post)
            .WithHeader(CoreClientConstants.ApiKeyHeader, _aiTrainerCoreConfiguration.ApiKey)
            .WithHeader(ApiConstants.CorrelationIdHeader)
            .RespondWithJson(mockedApiResponse);
        
        //Act
        var result = await _coreClientSimilaritySearch.TryInvokeAsync(input);
        
        //Assert
        Assert.NotNull(result);
        _httpTest
            .ShouldHaveCalled($"{_aiTrainerCoreConfiguration.BaseEndpoint}/api/faissrouter/similaritysearch")
            .WithVerb(HttpMethod.Post)
            .WithHeader(CoreClientConstants.ApiKeyHeader, _aiTrainerCoreConfiguration.ApiKey)
            .WithHeader(ApiConstants.CorrelationIdHeader);
    }
}