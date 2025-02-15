using System.Text;
using System.Text.Json;
using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Clients.Concrete;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.TestBase;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;

namespace AiTrainer.Web.CoreClient.Tests;

public class CoreClientCreateFaissStoreTests: CoreClientTestBase
{
    private readonly Mock<ILogger<CoreClientCreateFaissStore>> _mockLogger = new();
    private readonly CoreClientCreateFaissStore _coreClientCreateFaissStore;
    public CoreClientCreateFaissStoreTests()
    {
        SetUpBasicHttpContext();
        _coreClientCreateFaissStore = new CoreClientCreateFaissStore(
            _mockLogger.Object,
            new TestOptionsSnapshot<AiTrainerCoreConfiguration>(_aiTrainerCoreConfiguration).Object,
            _mockHttpContextAccessor.Object
        );
    }
    [Fact]
    public async Task CoreClientCreateFaissStore_Should_Build_Request_Correctly()
    {
        //Arrange
        var input = _fixture
            .Build<CreateFaissStoreInput>()
            .With(x => x.Documents,  _fixture.CreateMany<string>().ToArray())
            .Create();
        
        var stringJson = JsonSerializer.Serialize(input);
        await using var memStream = new MemoryStream(Encoding.UTF8.GetBytes(stringJson));

        var response = _fixture
            .Build<FaissStoreResponse>()
            .With(x => x.JsonDocStore, await JsonDocument.ParseAsync(memStream))
            .Create();
        var mockedApiResponse = new CoreResponse<FaissStoreResponse> { Data = response };
        
        _httpTest
            .ForCallsTo($"{_aiTrainerCoreConfiguration.BaseEndpoint}/api/faissrouter/createstore")
            .WithVerb(HttpMethod.Post)
            .WithHeader(CoreClientConstants.ApiKeyHeader, _aiTrainerCoreConfiguration.ApiKey)
            .WithHeader(ApiConstants.CorrelationIdHeader)
            .RespondWithJson(mockedApiResponse);
        
        //Act
        var result  = await _coreClientCreateFaissStore.TryInvokeAsync(input);
        
        //Assert
        Assert.NotNull(result);
        _httpTest
            .ShouldHaveCalled($"{_aiTrainerCoreConfiguration.BaseEndpoint}/api/faissrouter/createstore")
            .WithVerb(HttpMethod.Post)
            .WithHeader(CoreClientConstants.ApiKeyHeader, _aiTrainerCoreConfiguration.ApiKey)
            .WithHeader(ApiConstants.CorrelationIdHeader);
    }
}