using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Services.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.Common.Services.Concrete
{
    internal class ApiResolverService : IApiResolverService
    {
        private readonly IServiceProvider _serviceProvider;

        public ApiResolverService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T GetService<T>()
            where T : class =>
            _serviceProvider.GetService<T>()
            ?? throw new ArgumentNullException(ExceptionConstants.NoService);
    }
}
