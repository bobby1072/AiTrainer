using System.Linq.Expressions;

namespace AiTrainer.Web.Domain.Services.Abstract
{
    public interface IDomainServiceActionExecutor
    {
        Task<TReturn> ExecuteAsync<TService, TReturn>(
            Expression<Func<TService, Task<TReturn>>> serviceAction,
            string? serviceActionName = null
        )
            where TService : IDomainService;
        TReturn Execute<TService, TReturn>(
            Expression<Func<TService, TReturn>> serviceAction,
            string? serviceActionName = null
        )
            where TService : IDomainService;
    }
}
