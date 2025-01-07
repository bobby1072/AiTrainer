using System.Net;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Services.User.Abstract;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Api.SignalR.Hubs
{
    public class AiTrainerHub : Hub
    {
        public const string ConnectionIdUserIdCacheKeyPrefix = "userId-";
        private readonly IUserProcessingManager _userProcessingManager;
        private readonly ILogger<AiTrainerHub> _logger;
        public AiTrainerHub(IUserProcessingManager userProcessingManager, ILogger<AiTrainerHub> logger)
        {
            _userProcessingManager = userProcessingManager;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            var hubHttpContext = Context.GetHttpContext();
            var userAccessToken = hubHttpContext.GetAccessToken();
            
            var currentUser = await _userProcessingManager.TryGetUserFromCache(userAccessToken)
                              ?? throw new ApiException("Can't find user", HttpStatusCode.Unauthorized);
            
            
        }
    }
}
