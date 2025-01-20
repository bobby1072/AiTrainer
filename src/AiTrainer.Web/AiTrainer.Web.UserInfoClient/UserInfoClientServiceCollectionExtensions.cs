using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.UserInfoClient.Clients.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.UserInfoClient
{
    public static class UserInfoClientServiceCollectionExtensions
    {
        public static IServiceCollection AddUserInfoClient(this IServiceCollection services, IConfiguration config)
        {
            var userInfoSettingsSection =  config.GetSection(UserInfoClientConfiguration.Key);

            if (!userInfoSettingsSection.Exists())
            {
                throw new InvalidDataException(ExceptionConstants.MissingEnvVars);
            }
            
            services
                .Configure<UserInfoClientConfiguration>(userInfoSettingsSection)
                .AddScoped<IUserInfoClient, Clients.Concrete.UserInfoClient>();


            return services;
        }
    }
}
