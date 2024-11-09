using AiTrainer.Web.Domain.Services.User.Concrete;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.Domain.Services.User
{
    public static class UserServiceCollectionExtensions
    {
        public static IServiceCollection AddUserService(this IServiceCollection services)
        {
            services
                .AddScoped<IUserProcessingManager, UserProcessingManager>();
            return services;
        }
    }
}