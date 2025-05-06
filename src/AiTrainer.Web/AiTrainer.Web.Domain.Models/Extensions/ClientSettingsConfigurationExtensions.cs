using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Domain.Models.ApiModels.Response;

namespace AiTrainer.Web.Domain.Models.Extensions;

public static class ClientSettingsConfigurationExtensions
{
    public static ClientConfigurationResponse ToClientConfigurationResponse(
        this ClientSettingsConfiguration clientConfiguration)
    {
        return new ClientConfigurationResponse
        {
            Scope = clientConfiguration.Scope,
            AuthorityHost = clientConfiguration.AuthorityHost,
            AuthorityClientId = clientConfiguration.AuthorityClientId,
        };
    }
}