using System.Net;
using AiTrainer.Web.Common.Attributes;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Api.SignalR.Hubs
{
    // [RequireUserLogin]
    public class AiTrainerHub : Hub
    {
        public const string ConnectionIdUserIdCacheKeyPrefix = "userId-";
        private readonly IUserProcessingManager _userProcessingManager;
        private readonly ILogger<AiTrainerHub> _logger;
        private readonly ICachingService _cachingService; 
        public AiTrainerHub(IUserProcessingManager userProcessingManager, ILogger<AiTrainerHub> logger, ICachingService cachingService)
        {
            _userProcessingManager = userProcessingManager;
            _logger = logger;
            _cachingService = cachingService;
        }
        public async Task SendMessage(string message)
        {
            _logger.LogInformation("Sending message to {ConnectionId}", Context.ConnectionId);
            await Clients.Caller.SendAsync("ReceiveMessage", message);
        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            
            _logger.LogInformation("\n\n\n\nConnected to {ConnectionId}\n\n\n\n", Context.ConnectionId);

            // var hubHttpContext = Context.GetHttpContext();
            // var userAccessToken = hubHttpContext.GetAccessToken();
            //
            // var foundCachedUser =
            //     await _userProcessingManager.TryGetUserFromCache(
            //         userAccessToken
            //     ) ?? throw new ApiException("Can't find user", HttpStatusCode.Unauthorized);
            //
            // await _cachingService.SetObject($"{ConnectionIdUserIdCacheKeyPrefix}{foundCachedUser.Id}",
            //     Context.ConnectionId);
        }
    }
}
