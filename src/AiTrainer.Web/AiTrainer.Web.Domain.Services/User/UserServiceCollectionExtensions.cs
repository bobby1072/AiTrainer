using AiTrainer.Web.Domain.Services.User.Abstract;
using AiTrainer.Web.Domain.Services.User.Concrete;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.Domain.Services.User
{
    internal static class UserServiceCollectionExtensions
    {
        public static IServiceCollection AddUserServices(this IServiceCollection services)
        {
            services.AddTransient<IUserProcessingManager, UserProcessingManager>();

            return services;
        }
    }
}
