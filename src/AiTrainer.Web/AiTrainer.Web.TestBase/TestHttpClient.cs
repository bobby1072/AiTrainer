using BT.Common.FastArray.Proto;
using Xunit;

namespace AiTrainer.Web.TestBase;

public class TestHttpClient: HttpClient
{
    private string? _actualUri;
    private Dictionary<string, string?> _acutalHeaders = [];
    private HttpMethod? _actualMethod;
    public TestHttpClient(HttpMessageHandler handler) : base(handler)
    { }

    public void WasExpectedUrlCalled(string expectedUrl)
    {
        Assert.Equal(expectedUrl, _actualUri);
    }

    public void WasExpectedHeaderCalled(string headerName)
    {
        Assert.Contains(headerName, _acutalHeaders);
    }

    public void WasExpectedHeaderCalled(string headerName, string headerValue)
    {
        Assert.Single(_acutalHeaders, pair => headerName == pair.Key && headerValue == pair.Value);
    }

    public void WasExpectedHttpMethodUsed(HttpMethod method)
    {
        Assert.Equal(_actualMethod, method);
    }

    public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _actualUri = request.RequestUri?.ToString();
        _acutalHeaders = request.Headers.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault());
        _actualMethod = request.Method;
        
        return base.SendAsync(request, cancellationToken);
    }
}