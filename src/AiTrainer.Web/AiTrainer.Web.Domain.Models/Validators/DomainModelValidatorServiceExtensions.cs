using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.Domain.Models.Validators
{
    public static class DomainModelValidatorServiceExtensions
    {
        public static IServiceCollection AddDomainModelValidators(this IServiceCollection services)
        {
            services
                .AddSingleton<IValidator<User>, UserValidator>();
            
            return services;
        }
    }
}
