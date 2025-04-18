using System.Text.Json;

namespace AiTrainer.Web.Common
{
    public static class ApiConstants
    {
        public const string CorrelationIdHeader = "x-correlation-id-x";

        public static readonly JsonSerializerOptions DefaultCamelCaseSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }
}
