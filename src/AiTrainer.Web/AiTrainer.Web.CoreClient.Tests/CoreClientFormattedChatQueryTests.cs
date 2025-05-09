using System.Net;
using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.CoreClient.Clients.Concrete;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.TestBase;
using AutoFixture;
using Microsoft.Extensions.Logging.Abstractions;

namespace AiTrainer.Web.CoreClient.Tests;

public class CoreClientFormattedChatQueryTests: CoreClientTestBase
{
    public CoreClientFormattedChatQueryTests()
    {
        SetUpBasicHttpContext(true);
    }

    [Fact]
    public async Task CoreClientChunkDocument_Should_Build_Request_Correctly()
    {
        //Arrange
        var mockInput = _fixture.Create<FormattedChatQueryBuilder>();
        var expectedUri = $"{_aiTrainerCoreConfiguration.BaseEndpoint}/api/openairouter/formattedchatquery";
        var expectedResult = _fixture.Create<CoreResponse<CoreFormattedChatQueryResponse>>();

        using var httpClient = CreateDefaultCoreClientHttpClient(HttpStatusCode.OK, expectedResult);
        var service = new CoreClientFormattedChatQuery(
            new NullLogger<CoreClientFormattedChatQuery>(),
            new TestOptions<AiTrainerCoreConfiguration>(_aiTrainerCoreConfiguration),
            httpClient,
            _mockHttpContextAccessor.Object
        );
        
        //Act
        var result = await service.TryInvokeAsync(mockInput);
        
        //Assert
        httpClient.WasExpectedUrlCalled(expectedUri);
        httpClient.WasExpectedHeaderCalled(ApiConstants.CorrelationIdHeader);
        httpClient.WasExpectedHeaderCalled(CoreClientConstants.ApiKeyHeader, _aiTrainerCoreConfiguration.ApiKey);
        httpClient.WasExpectedHttpMethodUsed(HttpMethod.Post);
        Assert.Equal(expectedResult.Data?.Content, result?.Content);
    }
}