using Microsoft.AspNetCore.Http;

namespace AiTrainer.Web.Domain.Services.Abstract
{
    public interface IApiRequestHttpContextService
    {
        HttpContext HttpContext { get; }
        Guid? CorrelationId { get; }
        string? AccessToken { get; }

    }
}
