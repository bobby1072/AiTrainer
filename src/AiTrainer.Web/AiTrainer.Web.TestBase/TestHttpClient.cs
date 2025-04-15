namespace AiTrainer.Web.TestBase;

public class TestHttpClient: HttpClient
{
    private readonly string _expectedUrl;
    private string? _actualUri;

    public TestHttpClient(HttpMessageHandler handler, string expectedUrl) : base(handler)
    {
        _expectedUrl = expectedUrl;
    }

    public bool WasExpectedUrlCalled()
    {
        return _expectedUrl == _actualUri;
    }

    public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _actualUri = request.RequestUri?.ToString();
        return base.SendAsync(request, cancellationToken);
    }
}