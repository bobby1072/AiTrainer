using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.Concrete;
using AiTrainer.Web.Domain.Services.User;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.Domain.Services
{
    public static class DomainServicesServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {

            services
                .AddUserServices()
                .AddScoped<IDomainServiceActionExecutor, DomainServiceActionExecutor>()
                .AddScoped<ICachingService, DistributedCachingService>();

            return services;
        }
    }
}