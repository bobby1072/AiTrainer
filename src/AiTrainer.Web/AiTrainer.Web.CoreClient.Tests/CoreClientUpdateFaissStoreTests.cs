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

public class CoreClientUpdateFaissStoreTests: CoreClientTestBase
{
    public CoreClientUpdateFaissStoreTests()
    {
        SetUpBasicHttpContext();
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
        
        var httpClient = CreateDefaultCoreClientHttpClient(HttpStatusCode.OK,
            mockedApiResponse,
            $"{_aiTrainerCoreConfiguration.BaseEndpoint}/api/faissrouter/updatestore");
        var service = new CoreClientUpdateFaissStore(
            new NullLogger<CoreClientUpdateFaissStore>(),
            new TestOptionsSnapshot<AiTrainerCoreConfiguration>(_aiTrainerCoreConfiguration).Object,
            httpClient,
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