using AiTrainer.Web.Domain.Services.Workflow.Activities;
using AiTrainer.Web.Persistence.EntityFramework.Entities;
using AiTrainer.Web.Persistence.Models;
using BT.Common.WorkflowActivities;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.Domain.Services.User
{
    internal static class UserServiceCollectionExtensions
    {
        public static IServiceCollection AddUserServices(this IServiceCollection services)
        {
            services
                .AddDomainModelActivities<Models.User, Guid?>();

            services
                .AddActivity<SaveModelToDbActivity<Models.User, UserEntity, Guid>, IReadOnlyCollection<Models.User>?, DbSaveResult<Models.User>?>();

            return services;
        }
    }
}
