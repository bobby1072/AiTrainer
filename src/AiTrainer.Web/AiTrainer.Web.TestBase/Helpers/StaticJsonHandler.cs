using System.Net;
using System.Text;
using System.Text.Json;

namespace AiTrainer.Web.TestBase.Helpers;

public class StaticJsonHandler<T> : HttpMessageHandler
    where T : class
{
    private readonly T _json;
    private readonly HttpStatusCode _statusCode;
    private readonly JsonSerializerOptions? _jsonSerializerOptions;

    public StaticJsonHandler(
        T data,
        HttpStatusCode statusCode,
        JsonSerializerOptions? jsonSerializerOptions = null
    )
    {
        _json = data;
        _statusCode = statusCode;
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    protected sealed override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        var response = new HttpResponseMessage()
        {
            StatusCode = _statusCode,
            Content = new StringContent(
                JsonSerializer.Serialize(_json, _jsonSerializerOptions),
                Encoding.UTF8,
                "application/json"
            ),
        };

        return Task.FromResult(response);
    }
}
