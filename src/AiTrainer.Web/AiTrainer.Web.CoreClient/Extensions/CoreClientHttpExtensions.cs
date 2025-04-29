using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;

namespace AiTrainer.Web.CoreClient.Extensions;

internal static class CoreClientHttpExtensions
{
    public static JsonContent CreateApplicationJson<T>(T param, JsonSerializerOptions? serializerOptions = null) where T : class
    {
        return JsonContent.Create(param, MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json), serializerOptions);
    }
}