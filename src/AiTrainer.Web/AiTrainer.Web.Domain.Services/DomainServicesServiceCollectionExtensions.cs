using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.User;
using AiTrainer.Web.Domain.Services.Workflow.Activities;
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


        internal static IServiceCollection AddDomainModelActivities<TModel, TModelId>(this IServiceCollection services) where TModel : DomainModel<TModelId>
        {
            services
                .AddActivity<ValidateModelActivity<TModel?>, TModel?, ValidationResult?>()
                .AddActivity<ValidateDomainModelAgainstOriginalActivity<TModel, TModelId>, (TModel, TModel?), bool>();

            return services;
        }
    }
}
