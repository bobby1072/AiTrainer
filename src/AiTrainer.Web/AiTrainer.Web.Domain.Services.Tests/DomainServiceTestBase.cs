using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.Concrete;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
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
        protected readonly Mock<IHttpContextAccessor> MockContextAccessor = new();

        protected DomainServiceTestBase()
        {
            MockHttpContext.Setup(x => x.Request).Returns(MockHttpRequest.Object);
            MockHttpContext.Setup(x => x.Response).Returns(MockHttpResponse.Object);
            MockHttpContext.Setup(x => x.Request.Headers).Returns(HeaderDictionary);
            MockHttpContext.Setup(x => x.Response.Headers).Returns(HeaderDictionary);
            
            MockContextAccessor.Setup(x => x.HttpContext).Returns(MockHttpContext.Object);
        }

        protected void AddAccessTokenToRequestHeaders()
        {
            HeaderDictionary[HeaderNames.Authorization] = $"Bearer {Guid.NewGuid()}";
        }
    }
}
