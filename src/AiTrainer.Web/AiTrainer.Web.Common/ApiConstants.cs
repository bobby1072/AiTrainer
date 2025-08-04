using System.Text.Json;

namespace AiTrainer.Web.Common
{
    public static class ApiConstants
    {
        public static readonly JsonSerializerOptions DefaultCamelCaseSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }
}
