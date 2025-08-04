using System.Net;
using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.CoreClient.Clients.Concrete;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.TestBase;
using AutoFixture;
using BT.Common.Api.Helpers;
using Microsoft.Extensions.Logging.Abstractions;

namespace AiTrainer.Web.CoreClient.Tests;

public sealed class CoreClientChunkDocumentTests : CoreClientTestBase
{
    public CoreClientChunkDocumentTests()
    {
        SetUpBasicHttpContext(true);
    }

    [Fact]
    public async Task CoreClientChunkDocument_Should_Build_Request_Correctly()
    {
        //Arrange
        var mockInput = _fixture.Create<CoreDocumentToChunkInput>();
        var expectedUri = $"{_aiTrainerCoreConfiguration.BaseEndpoint}/api/chunkingrouter/chunkdocument";
        var expectedResult = _fixture.Create<CoreResponse<CoreChunkedDocumentResponse>>();

        using var httpClient = CreateDefaultCoreClientHttpClient(HttpStatusCode.OK, expectedResult);
        var service = new CoreClientChunkDocument(
            new NullLogger<CoreClientChunkDocument>(),
            httpClient,
            new TestOptions<AiTrainerCoreConfiguration>(_aiTrainerCoreConfiguration),
            _mockHttpContextAccessor.Object
        );
        
        //Act
        var result = await service.TryInvokeAsync(mockInput);
        
        //Assert
        httpClient.WasExpectedUrlCalled(expectedUri);
        httpClient.WasExpectedHeaderCalled(ApiConstants.CorrelationIdHeader);
        httpClient.WasExpectedHeaderCalled(CoreClientConstants.ApiKeyHeader, _aiTrainerCoreConfiguration.ApiKey);
        httpClient.WasExpectedHttpMethodUsed(HttpMethod.Post);
        Assert.NotNull(result);
        for (int i = 0; i < expectedResult.Data!.DocumentChunks.Count; i++)
        {
            var currentChunk = expectedResult.Data!.DocumentChunks.ElementAt(i);
            Assert.Equal(expectedResult.Data!.DocumentChunks.ElementAt(i), currentChunk);
        }
    }
}