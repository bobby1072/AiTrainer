using AiTrainer.Web.Domain.Models.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.Domain.Models.Extensions
{
    public static class DomainModelServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainModelServices(this IServiceCollection services)
        {
            services.AddDomainModelValidators();

            return services;
        }

        internal static IServiceCollection AddDomainModelValidators(
            this IServiceCollection services
        )
        {
            services.AddSingleton<IValidator<User>, UserValidator>();

            return services;
        }
    }
}
