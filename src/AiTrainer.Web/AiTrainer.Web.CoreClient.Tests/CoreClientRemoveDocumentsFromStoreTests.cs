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

public class CoreClientRemoveDocumentsFromStoreTests: CoreClientTestBase
{
    public CoreClientRemoveDocumentsFromStoreTests()
    {
        SetUpBasicHttpContext(true);
    }

    [Fact]
    public async Task CoreClientRemoveDocumentsFromStore_Should_Build_Request_Correctly()
    {
        //Arrange
        var stringJson = JsonSerializer.Serialize(new {Text = Faker.Lorem.Paragraph()});
        await using var inputMemStream = new MemoryStream(Encoding.UTF8.GetBytes(stringJson));
        await using var responseMemStream = new MemoryStream(Encoding.UTF8.GetBytes(stringJson));
        
        var input = _fixture
            .Build<CoreRemoveDocumentsFromStoreInput>()
            .With(x => x.DocStore, await JsonDocument.ParseAsync(inputMemStream))
            .With(x => x.FileInput, inputMemStream.ToArray())
            .Create();
        
        var response = _fixture
            .Build<CoreFaissStoreResponse>()
            .With(x => x.JsonDocStore, await JsonDocument.ParseAsync(responseMemStream))
            .Create();
        var mockedApiResponse = new CoreResponse<CoreFaissStoreResponse> { Data = response };

        var expecterdUrl = $"{_aiTrainerCoreConfiguration.BaseEndpoint}/api/faissrouter/removedocuments";
        using var httpClient = CreateDefaultCoreClientHttpClient(HttpStatusCode.OK,
            mockedApiResponse
            );
        var service = new CoreClientRemoveDocumentsFromStore(
            new NullLogger<CoreClientRemoveDocumentsFromStore>(),
            new TestOptions<AiTrainerCoreConfiguration>(_aiTrainerCoreConfiguration),
            _mockHttpContextAccessor.Object,
            httpClient
        );

        //Act
        var result = await service.TryInvokeAsync(input);
        
        //Assert
        httpClient.WasExpectedUrlCalled(expecterdUrl);
        httpClient.WasExpectedHeaderCalled(ApiConstants.CorrelationIdHeader);
        httpClient.WasExpectedHeaderCalled(CoreClientConstants.ApiKeyHeader, _aiTrainerCoreConfiguration.ApiKey);
        httpClient.WasExpectedHttpMethodUsed(HttpMethod.Post);
        Assert.NotNull(result);
        Assert.IsType<JsonDocument>(result.JsonDocStore);
        Assert.Equal(result.IndexFile, mockedApiResponse.Data.IndexFile);
    }
}