using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Services.Abstract;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.Domain.Services.Concrete;

internal class HealthProcessingManager: IHealthProcessingManager
{
    private readonly ICoreClient<CoreClientHealthResponse> _coreClient;
    private readonly ApplicationSettingsConfiguration _aiTrainerCoreConfiguration;
    public HealthProcessingManager(ICoreClient<CoreClientHealthResponse> coreClient, IOptionsSnapshot<ApplicationSettingsConfiguration> aiTrainerCoreConfiguration)
    {
        _coreClient = coreClient;
        _aiTrainerCoreConfiguration = aiTrainerCoreConfiguration.Value;
    }

    public async Task<Models.AiTrainerHealth> GetHealth()
    {
        var coreHealth = await _coreClient.TryInvokeAsync();

        return new Models.AiTrainerHealth
        {
            Name = _aiTrainerCoreConfiguration.Name,
            ReleaseVersion = _aiTrainerCoreConfiguration.ReleaseVersion,
            IsCoreHealthy = !string.IsNullOrEmpty(coreHealth?.Message),
        };
    }
}