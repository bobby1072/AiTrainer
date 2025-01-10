using AiTrainer.Web.Common.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Api.SignalR.Hubs
{
    public class AiTrainerHub : Hub
    {
        private readonly ILogger<AiTrainerHub> _logger;
        public AiTrainerHub(ILogger<AiTrainerHub> logger)
        {
            _logger = logger;
        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            var correlationId = Context.GetHttpContext()?.GetCorrelationId();
            
            _logger.LogInformation("Client connected with connectionId {ConnectionId} and correlationId {CorrelationId}", Context.ConnectionId, correlationId);
        }
    }
}
