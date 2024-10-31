using AiTrainer.Web.UserInfoClient.Clients.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.UserInfoClient
{
    public static class UserInfoClientServiceCollectionExtensions
    {
        public static IServiceCollection AddUserInfoClient(this IServiceCollection services)
        {
            services
                .AddHttpClient<IUserInfoClient, Clients.Concrete.UserInfoClient>();


            return services;
        }
    }
}
