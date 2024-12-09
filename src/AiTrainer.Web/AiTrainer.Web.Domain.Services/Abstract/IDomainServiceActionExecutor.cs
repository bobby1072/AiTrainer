namespace AiTrainer.Web.Domain.Services.Abstract
{
    public interface IDomainServiceActionExecutor
    {
        Task<TReturn> ExecuteAsync<TService, TReturn>(
            Func<TService, Task<TReturn>> serviceAction,
            string? serviceActionName = null
        )
            where TService : IDomainService;
        TReturn Execute<TService, TReturn>(
            Func<TService, TReturn> serviceAction,
            string? serviceActionName = null
        )
            where TService : IDomainService;
    }
}
