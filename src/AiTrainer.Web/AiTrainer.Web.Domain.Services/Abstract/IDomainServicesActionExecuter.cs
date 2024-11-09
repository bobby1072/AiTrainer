namespace AiTrainer.Web.Domain.Services.Abstract
{
    public interface IDomainServicesActionExecuter
    {
        Task<TReturn> ExecuteAsync<TService, TReturn>(Func<TService, Task<TReturn>> serviceAction) where TService : IDomainService;
        TReturn Execute<TService, TReturn>(Func<TService, TReturn> serviceAction) where TService : IDomainService;
    }
}
