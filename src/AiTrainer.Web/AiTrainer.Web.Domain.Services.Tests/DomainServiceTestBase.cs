using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.Concrete;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Moq;

namespace AiTrainer.Web.Domain.Services.Tests
{
    public abstract class DomainServiceTestBase
    {
        protected readonly Fixture _fixture = new();
        protected readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new();
        protected readonly Mock<HttpContext> _mockHttpContext = new();
        protected readonly Mock<HttpRequest> _mockHttpRequest = new();
        protected readonly Mock<HttpResponse> _mockHttpResponse = new();
        protected readonly Mock<IDomainServiceActionExecutor> _mockDomainServiceActionExecutor =
            new();
        protected readonly HeaderDictionary _headerDictionary = [];
        protected readonly ApiRequestHttpContextService _mockApiRequestService;

        protected DomainServiceTestBase()
        {
            _mockHttpContext.Setup(x => x.Request).Returns(_mockHttpRequest.Object);
            _mockHttpContext.Setup(x => x.Response).Returns(_mockHttpResponse.Object);
            _mockHttpContext.Setup(x => x.Request.Headers).Returns(_headerDictionary);
            _mockHttpContext.Setup(x => x.Response.Headers).Returns(_headerDictionary);
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(_mockHttpContext.Object);

            _mockApiRequestService = new ApiRequestHttpContextService(
                _mockHttpContextAccessor.Object
            );
        }
    }
}
