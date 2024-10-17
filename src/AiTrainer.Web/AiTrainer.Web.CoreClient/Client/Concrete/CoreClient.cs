using AiTrainer.Web.CoreClient.Client.Abstract;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.CoreClient.Client.Concrete
{
    public partial class CoreClient: ICoreClient
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        public CoreClient(HttpClient httpClient, ILogger<CoreClient> logger) { 
            _httpClient = httpClient;
            _logger = logger;
        }

    }
}
