using AiTrainer.Web.Domain.Models.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.Domain.Models
{
    public static class DomainModelServiceExtensions
    {
        public static IServiceCollection AddDomainModelValidators(this IServiceCollection services)
        {
            services
                .AddSingleton<IValidator<User>, UserValidator>();

            return services;
        }
    }
}
