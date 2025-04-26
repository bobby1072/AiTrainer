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

public class CoreClientCreateFaissStoreTests: CoreClientTestBase
{
    public CoreClientCreateFaissStoreTests()
    {
        SetUpBasicHttpContext();
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
            .Create();
        var mockedApiResponse = new CoreResponse<CoreFaissStoreResponse> { Data = response };
        
        var httpClient = CreateDefaultCoreClientHttpClient(HttpStatusCode.OK, mockedApiResponse, expectedUrl);
        var service = new CoreClientCreateFaissStore(
            new NullLogger<CoreClientCreateFaissStore>(),
            httpClient,
            new TestOptions<AiTrainerCoreConfiguration>(_aiTrainerCoreConfiguration),
            _mockHttpContextAccessor.Object
        );
        
        //Act
        var result = await service.TryInvokeAsync(input);
        
        //Assert
        Assert.True(httpClient.WasExpectedUrlCalled());
        Assert.NotNull(result);
        Assert.IsType<JsonDocument>(result.JsonDocStore);
        Assert.Equal(result.IndexFile, mockedApiResponse.Data.IndexFile);
    }
}