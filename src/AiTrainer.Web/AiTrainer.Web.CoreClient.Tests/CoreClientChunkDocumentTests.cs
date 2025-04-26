using System.Net;
using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.CoreClient.Clients.Concrete;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.TestBase;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;

namespace AiTrainer.Web.CoreClient.Tests;

public class CoreClientChunkDocumentTests : CoreClientTestBase
{
    public CoreClientChunkDocumentTests()
    {
        SetUpBasicHttpContext();
    }

    [Fact]
    public async Task CoreClientChunkDocument_Should_Build_Request_Correctly()
    {
        //Arrange
        var mockInput = _fixture.Create<CoreDocumentToChunkInput>();
        var expectedUri = "http://localhost:5000/api/chunkingrouter/chunkdocument";
        var expectedResult = _fixture.Create<CoreResponse<CoreChunkedDocumentResponse>>();

        var httpClient = CreateDefaultCoreClientHttpClient(HttpStatusCode.OK, expectedResult, expectedUri);
        var service = new CoreClientChunkDocument(
            Mock.Of<ILogger<CoreClientChunkDocument>>(),
            httpClient,
            new TestOptions<AiTrainerCoreConfiguration>(_aiTrainerCoreConfiguration),
            _mockHttpContextAccessor.Object
        );
        
        //Act
        var result = await service.TryInvokeAsync(mockInput);
        
        //Assert
        Assert.True(httpClient.WasExpectedUrlCalled());
        Assert.NotNull(result);
        for (int i = 0; i < expectedResult.Data!.DocumentChunks.Count; i++)
        {
            var currentChunk = expectedResult.Data!.DocumentChunks.ElementAt(i);
            Assert.Equal(expectedResult.Data!.DocumentChunks.ElementAt(i), currentChunk);
        }
    }
}