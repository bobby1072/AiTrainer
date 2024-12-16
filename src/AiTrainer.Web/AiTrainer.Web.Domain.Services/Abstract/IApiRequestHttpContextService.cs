
namespace AiTrainer.Web.Domain.Services.Abstract
{
    public interface IApiRequestHttpContextService
    {
        Guid? CorrelationId { get; }
        string AccessToken { get; }

    }
}
