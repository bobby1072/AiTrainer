using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.Common.Workflow.Activities;
using BT.Common.WorkflowActivities;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.Domain.Services
{
    public static class DomainServicesSeviceCollectionExtensions
    {
        private static readonly Type _domainModelType = typeof(DomainModel<object>);
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services
                .AddUserServices();

            return services;
        }


        private static IServiceCollection AddUserServices(this IServiceCollection services)
        {
            services
                .AddActivity<ValidateModelActivity<Models.User>, Models.User, ValidationResult>()
                .AddActivity<ValidateDomainModelAgainstOriginalActivity<Models.User, Guid?>, (Models.User, Models.User), bool>();

            return services;
        }
    }
}
