using System.Net;
using System.Text;
using System.Text.Json;
using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.CoreClient.Clients.Concrete;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.TestBase;
using AutoFixture;
using Microsoft.Extensions.Logging.Abstractions;

namespace AiTrainer.Web.CoreClient.Tests;

public sealed class CoreClientCreateFaissStoreTests: CoreClientTestBase
{
    public CoreClientCreateFaissStoreTests()
    {
        SetUpBasicHttpContext(true);
    }

    [Fact]
    public async Task CoreClientCreateFaissStore_Should_Build_Request_Correctly()
    {
        //Arrange
        var input = _fixture
            .Build<CoreCreateFaissStoreInput>()
            .With(x => x.Documents,  _fixture.CreateMany<CoreCreateFaissStoreInputDocument>().ToArray())
            .Create();
        var expectedUrl = $"{_aiTrainerCoreConfiguration.BaseEndpoint}/api/faissrouter/createstore";
        
        var stringJson = JsonSerializer.Serialize(input);
        await using var memStream = new MemoryStream(Encoding.UTF8.GetBytes(stringJson));

        var response = _fixture
            .Build<CoreFaissStoreResponse>()
            .With(x => x.JsonDocStore, await JsonDocument.ParseAsync(memStream))
            .With(x => x.IndexFile, memStream.ToArray())
            .Create();
        var mockedApiResponse = new CoreResponse<CoreFaissStoreResponse> { Data = response };
        
        using var httpClient = CreateDefaultCoreClientHttpClient(HttpStatusCode.OK, mockedApiResponse);
        var service = new CoreClientCreateFaissStore(
            new NullLogger<CoreClientCreateFaissStore>(),
            httpClient,
            new TestOptions<AiTrainerCoreConfiguration>(_aiTrainerCoreConfiguration),
            _mockHttpContextAccessor.Object
        );
        
        //Act
        var result = await service.TryInvokeAsync(input);
        
        //Assert
        httpClient.WasExpectedUrlCalled(expectedUrl);
        httpClient.WasExpectedHeaderCalled(ApiConstants.CorrelationIdHeader);
        httpClient.WasExpectedHeaderCalled(CoreClientConstants.ApiKeyHeader, _aiTrainerCoreConfiguration.ApiKey);
        httpClient.WasExpectedHttpMethodUsed(HttpMethod.Post);
        Assert.NotNull(result);
        Assert.IsType<JsonDocument>(result.JsonDocStore);
        Assert.Equal(result.IndexFile, mockedApiResponse.Data.IndexFile);
    }
}