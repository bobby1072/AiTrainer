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

public class CoreClientChunkDocumentTests: CoreClientTestBase
{
    private readonly Mock<ILogger<CoreClientChunkDocument>> _mockLogger = new();
    private readonly CoreClientChunkDocument _clientChunkDocument;
    public CoreClientChunkDocumentTests()
    {
        SetUpBasicHttpContext();
        _clientChunkDocument = new CoreClientChunkDocument(
            _mockLogger.Object,
            new TestOptionsSnapshot<AiTrainerCoreConfiguration>(_aiTrainerCoreConfiguration).Object,
            _mockHttpContextAccessor.Object
        );
    }
    [Fact]
    public async Task CoreClientChunkDocument_Should_Build_Request_Correctly()
    {
        //Arrange
        var documentToChunk = _fixture
            .Build<DocumentToChunkInput>()
            .With(x => x.DocumentText, _fixture.CreateMany<string>().ToArray())
            .Create();

        var chunkedDoc = _fixture
            .Build<ChunkedDocumentResponse>()
            .With(x => x.DocumentChunks, _fixture.CreateMany<string>().ToArray())
            .Create();
        var mockedApiResponse = new CoreResponse<ChunkedDocumentResponse> { Data = chunkedDoc };
        
        _httpTest
            .ForCallsTo($"{_aiTrainerCoreConfiguration.BaseEndpoint}/api/chunkingrouter/chunkdocument")
            .WithVerb(HttpMethod.Post)
            .WithHeader(CoreClientConstants.ApiKeyHeader, _aiTrainerCoreConfiguration.ApiKey)
            .WithHeader(ApiConstants.CorrelationIdHeader)
            .RespondWithJson(mockedApiResponse);

        //Act
        var result = await _clientChunkDocument.TryInvokeAsync(documentToChunk);

        //Assert
        Assert.NotNull(result);
        _httpTest
            .ShouldHaveCalled($"{_aiTrainerCoreConfiguration.BaseEndpoint}/api/chunkingrouter/chunkdocument")
            .WithVerb(HttpMethod.Post)
            .WithHeader(CoreClientConstants.ApiKeyHeader, _aiTrainerCoreConfiguration.ApiKey)
            .WithHeader(ApiConstants.CorrelationIdHeader)
            .WithRequestJson(documentToChunk);
    }
}