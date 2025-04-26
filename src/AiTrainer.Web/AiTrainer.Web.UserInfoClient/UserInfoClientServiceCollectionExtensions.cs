using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.UserInfoClient.Clients.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.UserInfoClient
{
    public static class UserInfoClientServiceCollectionExtensions
    {
        public static IServiceCollection AddUserInfoClient(this IServiceCollection services, IConfiguration config)
        {
            var userInfoSettingsSection =  config.GetSection(UserInfoClientConfiguration.Key);

            if (userInfoSettingsSection is null)
            {
                throw new InvalidDataException(ExceptionConstants.MissingEnvVars);
            }
            
            services
                .Configure<UserInfoClientConfiguration>(userInfoSettingsSection);
            
            services
                .AddHttpClient<IUserInfoClient, Clients.Concrete.UserInfoClient>((sp, cli) =>
                {
                    var infoConfig = sp.GetRequiredService<IOptions<UserInfoClientConfiguration>>();
                    
                    cli.Timeout = TimeSpan.FromSeconds(infoConfig.Value.TimeoutInSeconds ?? 5);
                });


            return services;
        }
    }
}
