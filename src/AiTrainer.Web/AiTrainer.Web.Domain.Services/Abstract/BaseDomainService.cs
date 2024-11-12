namespace AiTrainer.Web.Domain.Services.Abstract
{
    public abstract class BaseDomainService : IDomainService
    {
        protected readonly IDomainServiceActionExecutor _domainServiceActionExecutor;

        protected BaseDomainService(IDomainServiceActionExecutor domainServiceActionExecutor)
        {
            _domainServiceActionExecutor = domainServiceActionExecutor;
        }
    }
}
