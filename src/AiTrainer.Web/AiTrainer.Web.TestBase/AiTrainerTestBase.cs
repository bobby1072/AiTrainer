using AiTrainer.Web.Common;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Moq;

namespace AiTrainer.Web.TestBase;

public abstract class AiTrainerTestBase
{
    protected readonly Fixture _fixture = new();
    protected readonly Mock<HttpContext> _mockHttpContext = new();
    protected readonly Mock<HttpRequest> _mockHttpRequest = new();
    protected readonly Mock<HttpResponse> _mockHttpResponse = new();
    protected readonly HeaderDictionary _headerDictionary = [];
    protected readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new();

    protected void SetUpBasicHttpContext(bool addCorrelationId = false)
    {   
        if (addCorrelationId)
        {
            _headerDictionary.Add(ApiConstants.CorrelationIdHeader, Guid.NewGuid().ToString());
        }
        _mockHttpContext.Setup(x => x.Request).Returns(_mockHttpRequest.Object);
        _mockHttpContext.Setup(x => x.Response).Returns(_mockHttpResponse.Object);
        _mockHttpContext.Setup(x => x.Request.Headers).Returns(_headerDictionary);
        _mockHttpContext.Setup(x => x.Response.Headers).Returns(_headerDictionary);
            
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(_mockHttpContext.Object);

    }
}