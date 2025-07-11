﻿using System.Linq.Expressions;

namespace AiTrainer.Web.Domain.Services.Abstract
{
    public interface IHttpDomainServiceActionExecutor
    {
        Task ExecuteAsync<TService>(
            Expression<Func<TService, Task>> serviceAction,
            string? serviceActionName = null
        )
            where TService : IDomainProcessingManager;
        Task<TReturn> ExecuteAsync<TService, TReturn>(
            Expression<Func<TService, Task<TReturn>>> serviceAction,
            string? serviceActionName = null
        )
            where TService : IDomainProcessingManager;
    }
}
