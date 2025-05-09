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

public class CoreClientUpdateFaissStoreTests: CoreClientTestBase
{
    public CoreClientUpdateFaissStoreTests()
    {
        SetUpBasicHttpContext(true);
    }

    [Fact]
    public async Task CoreClientUpdateFaissStore_Should_Build_Request_Correctly()
    {
        //Arrange
        var stringJson = JsonSerializer.Serialize(new {Text = Faker.Lorem.Paragraph()});
        await using var inputMemStream = new MemoryStream(Encoding.UTF8.GetBytes(stringJson));
        await using var responseMemStream = new MemoryStream(Encoding.UTF8.GetBytes(stringJson));
        
        var input = _fixture
            .Build<CoreUpdateFaissStoreInput>()
            .With(x => x.DocStore, await JsonDocument.ParseAsync(inputMemStream))
            .Create();
        
        var response = _fixture
            .Build<CoreFaissStoreResponse>()
            .With(x => x.JsonDocStore, await JsonDocument.ParseAsync(responseMemStream))
            .Create();
        var mockedApiResponse = new CoreResponse<CoreFaissStoreResponse> { Data = response };
        var expectedUrl = $"{_aiTrainerCoreConfiguration.BaseEndpoint}/api/faissrouter/updatestore";
        using var httpClient = CreateDefaultCoreClientHttpClient(HttpStatusCode.OK,
            mockedApiResponse);
        var service = new CoreClientUpdateFaissStore(
            new NullLogger<CoreClientUpdateFaissStore>(),
            new TestOptions<AiTrainerCoreConfiguration>(_aiTrainerCoreConfiguration),
            httpClient,
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