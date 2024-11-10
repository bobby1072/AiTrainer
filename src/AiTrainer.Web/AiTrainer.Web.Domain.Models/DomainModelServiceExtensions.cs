using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AiTrainer.Web.Domain.Models
{
    public static class DomainModelServiceExtensions
    {
        public static IServiceCollection AddDomainModelServices(this IServiceCollection services) 
        {
            services.AddDomainModelValidators();

            return services;
        }
        internal static IServiceCollection AddDomainModelValidators(this IServiceCollection services)
        {

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
