using AiTrainer.Web.Common;
using AiTrainer.Web.CoreClient.Client.Abstract;
using AiTrainer.Web.CoreClient.Service.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.CoreClient.Service.Concrete
{
    internal class CoreClientServiceProvider : ICoreClientServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CoreClientServiceProvider> _logger;

        public CoreClientServiceProvider(
            IServiceProvider serviceProvider,
            ILogger<CoreClientServiceProvider> logger
        )
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task<TReturn> ExecuteAsync<TParam, TReturn>(TParam param)
            where TReturn : class
        {
            var client = GetClient<TParam, TReturn>();

            return client.InvokeAsync(param);
        }

        public Task<TReturn> ExecuteAsync<TReturn>()
            where TReturn : class
        {
            var client = GetClient<TReturn>();

            return client.InvokeAsync();
        }

        public Task<TReturn?> TryExecuteAsync<TParam, TReturn>(TParam param)
            where TReturn : class
        {
            var client = GetClient<TParam, TReturn>();
            try
            {
                return client.TryInvokeAsync(param);
            }
            catch
            {
                return null!;
            }
        }

        public Task<TReturn?> TryExecuteAsync<TReturn>()
            where TReturn : class
        {
            var client = GetClient<TReturn>();
            try
            {
                return client.TryInvokeAsync();
            }
            catch
            {
                return null!;
            }
        }

        private ICoreClient<TParam, TReturn> GetClient<TParam, TReturn>()
            where TReturn : class
        {
            var client = _serviceProvider.GetService<ICoreClient<TParam, TReturn>>();

            if (client is null)
            {
                var noServiceException = new InvalidOperationException(
                    ExceptionConstants.NoService
                );
                _logger.LogError(
                    noServiceException,
                    "Couldn't resolve client for param type {TParam} and return type {TReturn}",
                    nameof(TReturn),
                    nameof(TReturn)
                );
                throw noServiceException;
            }
            return client;
        }

        private ICoreClient<TReturn> GetClient<TReturn>()
            where TReturn : class
        {
            var client = _serviceProvider.GetService<ICoreClient<TReturn>>();

            if (client is null)
            {
                var noServiceException = new InvalidOperationException(
                    ExceptionConstants.NoService
                );
                _logger.LogError(
                    noServiceException,
                    "Couldn't resolve client for return type {TReturn}",
                    nameof(TReturn)
                );
                throw noServiceException;
            }
            return client;
        }
    }
}
