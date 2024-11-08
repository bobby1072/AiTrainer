using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.User;
using AiTrainer.Web.Domain.Services.Workflow.Activities;
using AiTrainer.Web.Persistence.EntityFramework.Entities;
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
                .AddActivity<ValidateModelActivity<TModel>, ValidateModelActivityContextItem<TModel>, ValidateModelActivityReturnItem>()
                .AddActivity<ValidateDomainModelAgainstOriginalActivity<TModel, TModelId>, ValidateDomainModelAgainstOriginalActivityContextItem<TModel, TModelId>, ValidateDomainModelAgainstOriginalActivityReturnItem>();

            return services;
        }
        internal static IServiceCollection AddDefaultDbActivities<TModel, TEnt, TEntId>(this IServiceCollection services)
        where TEnt : BaseEntity<TEntId, TModel>
        where TModel : class
        {
            services
                .AddActivity<SaveModelToDbActivity<TModel, TEnt, TEntId>, SaveModelToDbActivityContextItem<TModel>, SaveModelToDbActivityReturnItem<TModel>>();


            return services;
        }
    }
}
