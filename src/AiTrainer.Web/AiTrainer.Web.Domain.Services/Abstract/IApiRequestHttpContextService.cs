using Microsoft.AspNetCore.Http;

namespace AiTrainer.Web.Domain.Services.Abstract
{
    public interface IApiRequestHttpContextService
    {
        Guid? CorrelationId { get; }
        Guid DeviceToken { get; }
    }
}
