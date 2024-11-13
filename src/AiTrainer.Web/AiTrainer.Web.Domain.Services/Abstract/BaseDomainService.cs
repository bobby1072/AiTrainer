namespace AiTrainer.Web.Domain.Services.Abstract
{
    public abstract class BaseDomainService : IDomainService
    {
        protected readonly IDomainServiceActionExecutor _domainServiceActionExecutor;
        protected readonly IApiRequestHttpContextService _apiRequestHttpContextService;
        protected BaseDomainService(IDomainServiceActionExecutor domainServiceActionExecutor, IApiRequestHttpContextService apiRequestHttpContextService)
        {
            _domainServiceActionExecutor = domainServiceActionExecutor;
            _apiRequestHttpContextService = apiRequestHttpContextService;
        }
    }
}
