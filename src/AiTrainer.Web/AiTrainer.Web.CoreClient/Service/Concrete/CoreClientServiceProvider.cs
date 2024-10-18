using AiTrainer.Web.Common;
using AiTrainer.Web.CoreClient.Client.Abstract;
using AiTrainer.Web.CoreClient.Service.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.CoreClient.Service
{
    internal class CoreClientServiceProvider : ICoreClientServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public CoreClientServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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
            try
            {
                var client = GetClient<TParam, TReturn>();

                return client.TryInvokeAsync(param);
            }
            catch
            {
                return null;
            }
        }

        public Task<TReturn?> TryExecuteAsync<TReturn>()
            where TReturn : class
        {
            try
            {
                var client = GetClient<TReturn>();

                return client.TryInvokeAsync();
            }
            catch
            {
                return null;
            }
        }

        private ICoreClient<TParam, TReturn> GetClient<TParam, TReturn>()
            where TReturn : class
        {
            var client =
                _serviceProvider.GetService<ICoreClient<TParam, TReturn>>()
                ?? throw new InvalidOperationException(ExceptionConstants.NoService);
            return client;
        }

        private ICoreClient<TReturn> GetClient<TReturn>()
            where TReturn : class
        {
            var client =
                _serviceProvider.GetService<ICoreClient<TReturn>>()
                ?? throw new InvalidOperationException(ExceptionConstants.NoService);
            return client;
        }
    }
}
