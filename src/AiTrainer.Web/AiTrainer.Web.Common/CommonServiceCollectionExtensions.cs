using AiTrainer.Web.Common.Services.Abstract;
using AiTrainer.Web.Common.Services.Concrete;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.Common
{
    public static class CommonServiceCollectionExtensions
    {
        public static IServiceCollection AddCommon(this IServiceCollection services)
        {
            services.AddScoped<IApiResolverService, ApiResolverService>();
            return services;
        }
    }
}
