using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.UserInfoClient.Clients.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
                .AddScoped<IUserInfoClient>
                    (sp => new Clients.Concrete.UserInfoClient(
                        sp.GetRequiredService<IOptionsSnapshot<ClientSettingsConfiguration>>(),
                        sp.GetRequiredService<IHttpContextAccessor>(),
                        sp.GetRequiredService<IOptionsSnapshot<UserInfoClientConfiguration>>(),
                        sp.GetRequiredService<ILoggerFactory>().CreateLogger<Clients.Concrete.UserInfoClient>(),
                        ApiConstants.DefaultCamelFlurlJsonSerializer
                    ));


            return services;
        }
    }
}
