using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Client.Abstract;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.CoreClient.Client.Concrete
{
    public partial class CoreClient: ICoreClient
    {
        private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        public CoreClient(HttpClient httpClient, ILogger<CoreClient> logger, IOptions<AiTrainerCoreConfiguration> aiTrainerCoreConfig) { 
            _httpClient = httpClient;
            _logger = logger;
            _aiTrainerCoreConfiguration = aiTrainerCoreConfig.Value;
        }


    }
}
