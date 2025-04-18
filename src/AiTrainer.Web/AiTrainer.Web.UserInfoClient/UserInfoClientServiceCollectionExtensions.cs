using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.UserInfoClient.Clients.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.UserInfoClient
{
    public static class UserInfoClientServiceCollectionExtensions
    {
        public static IServiceCollection AddUserInfoClient(this IServiceCollection services, IConfiguration config)
        {
            var userInfoSettingsSection =  config.GetSection(UserInfoClientConfiguration.Key).Get<UserInfoClientConfiguration>();

            if (userInfoSettingsSection is null)
            {
                throw new InvalidDataException(ExceptionConstants.MissingEnvVars);
            }
            
            services
                .AddOptions<UserInfoClientConfiguration>(UserInfoClientConfiguration.Key);
            
            services
                .AddHttpClient<IUserInfoClient, Clients.Concrete.UserInfoClient>(cli =>
                {
                    cli.Timeout = TimeSpan.FromSeconds(userInfoSettingsSection.TimeoutInSeconds ?? 5);
                });


            return services;
        }
    }
}
