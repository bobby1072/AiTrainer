using AiTrainer.Web.Domain.Services.Common.Workflow.Activities;
using BT.Common.WorkflowActivities;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.Domain.Services
{
    public static class DomainServicesSeviceCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services
                .AddUserServices();

            return services;
        }


        public static IServiceCollection AddUserServices(this IServiceCollection services)
        {
            services
                .AddActivity<ValidateModelActivity<Models.User>, Models.User, ValidationResult>();

            return services;
        }
    }
}
