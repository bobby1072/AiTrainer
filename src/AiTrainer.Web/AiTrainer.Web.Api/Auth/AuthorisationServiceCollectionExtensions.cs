using AiTrainer.Web.Common.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AiTrainer.Web.Api.Auth
{
    public static class AuthorisationServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthorizationServices(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment environment
        )
        {
            var clientConfig = configuration.GetSection(ClientSettingsConfiguration.Key);

            if (!clientConfig.Exists())
            {
                throw new Exception("ClientSettingsConfiguration not found in configuration");
            }

            var issuerHost = clientConfig.GetValue<string>("InternalAuthorityHost");

            services
                .AddAuthorization()
                .Configure<ClientSettingsConfiguration>(clientConfig)
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = issuerHost;
                    options.RequireHttpsMetadata = !environment.IsDevelopment();
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = false,
                        ValidIssuer = issuerHost,
                    };
                });
            ;

            return services;
        }
    }
}
