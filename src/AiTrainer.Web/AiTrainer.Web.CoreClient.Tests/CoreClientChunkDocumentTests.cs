using System.Net;
using System.Net.Http.Json;
using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.CoreClient.Clients.Concrete;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.TestBase;
using AutoFixture;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;

namespace AiTrainer.Web.CoreClient.Tests;

public class CoreClientChunkDocumentTests : CoreClientTestBase
{
    private readonly CoreClientChunkDocument _clientChunkDocument;
    public CoreClientChunkDocumentTests()
    {
        SetUpBasicHttpContext();
    }

    [Fact]
    public async Task CoreClientChunkDocument_Should_Build_Request_Correctly()
    {
        //Arrange
        var fixture = new Fixture().AddMockHttp();

        var mockInput = fixture.Create<CoreDocumentToChunkInput>();
        var expectedUri = "http://localhost:5000/api/chunkingrouter/chunkdocument";
        var expectedResult = fixture.Create<CoreResponse<CoreChunkedDocumentResponse>>();

        var handler = fixture.Freeze<MockHttpMessageHandler>();
        handler.When(HttpMethod.Post, expectedUri)
            .Respond(HttpStatusCode.OK, JsonContent.Create(expectedResult, null, ApiConstants.DefaultCamelCaseSerializerOptions));

        var service = new CoreClientChunkDocument(
            Mock.Of<ILogger<CoreClientChunkDocument>>(),
            handler.ToHttpClient(),
            new TestOptionsSnapshot<AiTrainerCoreConfiguration>(_aiTrainerCoreConfiguration).Object,
            _mockHttpContextAccessor.Object
        );
        
        //Act
        var result = await service.TryInvokeAsync(mockInput);
        
        //Assert
        Assert.Equal(result, expectedResult.Data);
    }
}