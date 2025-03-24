using System.Text;
using System.Text.Json;
using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Clients.Concrete;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.TestBase;
using AutoFixture;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace AiTrainer.Web.CoreClient.Tests;

public class CoreClientUpdateFaissStoreTests: CoreClientTestBase
{
    private readonly Mock<ILogger<CoreClientUpdateFaissStore>> _mockLogger = new();
    private readonly CoreClientUpdateFaissStore _coreClientUpdateFaissStore;

    public CoreClientUpdateFaissStoreTests()
    {
        SetUpBasicHttpContext();
        
        _httpTest.WithSettings(x =>
        {
            x.JsonSerializer = ApiConstants.DefaultCamelFlurlJsonSerializer;
        });
        
        _coreClientUpdateFaissStore = new CoreClientUpdateFaissStore(
            _mockLogger.Object,
            new TestOptionsSnapshot<AiTrainerCoreConfiguration>(_aiTrainerCoreConfiguration).Object,
            _mockHttpContextAccessor.Object,
            ApiConstants.DefaultCamelFlurlJsonSerializer
        );
    }
    [Fact]
    public async Task CoreClientUpdateFaissStore_Should_Build_Request_Correctly()
    {
        //Arrange
        var stringJson = JsonSerializer.Serialize(new {Text = Faker.Lorem.Paragraph()});
        await using var inputMemStream = new MemoryStream(Encoding.UTF8.GetBytes(stringJson));
        await using var responseMemStream = new MemoryStream(Encoding.UTF8.GetBytes(stringJson));
        
        var input = _fixture
            .Build<UpdateFaissStoreInput>()
            .With(x => x.DocStore, await JsonDocument.ParseAsync(inputMemStream))
            .Create();
        
        var response = _fixture
            .Build<FaissStoreResponse>()
            .With(x => x.JsonDocStore, await JsonDocument.ParseAsync(responseMemStream))
            .Create();
        var mockedApiResponse = new CoreResponse<FaissStoreResponse> { Data = response };
        
        _httpTest
            .ForCallsTo($"{_aiTrainerCoreConfiguration.BaseEndpoint}/api/faissrouter/updatestore")
            .WithVerb(HttpMethod.Post)
            .WithHeader(CoreClientConstants.ApiKeyHeader, _aiTrainerCoreConfiguration.ApiKey)
            .WithHeader(ApiConstants.CorrelationIdHeader)
            .RespondWithJson(mockedApiResponse);
        
        //Act
        var result = await _coreClientUpdateFaissStore.TryInvokeAsync(input);
        
        //Assert
        Assert.NotNull(result);
        _httpTest
            .ShouldHaveCalled($"{_aiTrainerCoreConfiguration.BaseEndpoint}/api/faissrouter/updatestore")
            .WithVerb(HttpMethod.Post)
            .WithHeader(CoreClientConstants.ApiKeyHeader, _aiTrainerCoreConfiguration.ApiKey)
            .WithHeader(ApiConstants.CorrelationIdHeader);
    }
}