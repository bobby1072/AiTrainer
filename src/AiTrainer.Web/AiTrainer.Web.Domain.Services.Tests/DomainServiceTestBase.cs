using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.Concrete;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Moq;

namespace AiTrainer.Web.Domain.Services.Tests
{
    public abstract class DomainServiceTestBase
    {
        protected readonly Fixture Fixture = new();
        protected readonly Mock<HttpContext> MockHttpContext = new();
        protected readonly Mock<HttpRequest> MockHttpRequest = new();
        protected readonly Mock<HttpResponse> MockHttpResponse = new();
        protected readonly Mock<IDomainServiceActionExecutor> MockDomainServiceActionExecutor =
            new();
        protected readonly HeaderDictionary HeaderDictionary = [];
        protected readonly ApiRequestHttpContextService MockApiRequestService;

        protected DomainServiceTestBase()
        {
            MockHttpContext.Setup(x => x.Request).Returns(MockHttpRequest.Object);
            MockHttpContext.Setup(x => x.Response).Returns(MockHttpResponse.Object);
            MockHttpContext.Setup(x => x.Request.Headers).Returns(HeaderDictionary);
            MockHttpContext.Setup(x => x.Response.Headers).Returns(HeaderDictionary);

            MockApiRequestService = new ApiRequestHttpContextService(
                    MockHttpContext.Object
                );
        }
    }
}
