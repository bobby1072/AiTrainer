using System.Net;
using AiTrainer.Web.Api.SignalR.Models;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Api.SignalR.Hubs
{
    public class AiTrainerHub : Hub
    {
        public const string ConnectionIdUserIdCacheKeyPrefix = "userId-";
        private readonly IDomainServiceActionExecutor _domainService;
        private readonly ILogger<AiTrainerHub> _logger;
        private readonly ICachingService _cachingService;

        public AiTrainerHub(
            ILogger<AiTrainerHub> logger,
            ICachingService cachingService,
            IDomainServiceActionExecutor domainService
        )
        {
            _domainService = domainService;
            _logger = logger;
            _cachingService = cachingService;
        }

        public async Task AuthenticateUser(JsonWebTokenHubInput tokenInput)
        {
            var hubHttpContext = Context.GetHttpContext();
            try
            {
                var currentUser =
                    await _domainService.ExecuteAsync<IUserProcessingManager, User>(userProcessingManager => userProcessingManager.SaveAndCacheUser(tokenInput.AccessToken))
                    ?? throw new ApiException("Can't find user", HttpStatusCode.Unauthorized);

                await _cachingService.SetObject(
                    $"{ConnectionIdUserIdCacheKeyPrefix}{currentUser.Id}",
                    Context.ConnectionId
                );
                
                await Clients.Client(Context.ConnectionId).SendAsync(SignalRConstants.AuthSuccessEventMessage, new SignalRClientEvent<User>{Data = currentUser});
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "Exception occured while authenticating user with signal R for connectionId {ConnectionId} and correlationId {CorrelationId}",
                    Context.ConnectionId,
                    hubHttpContext?.GetCorrelationId().ToString() ?? ""
                );

                await Clients
                    .Client(Context.ConnectionId)
                    .SendAsync(SignalRConstants.ErrorEventMessage, new SignalRClientEvent {
                    ExceptionMessage = ExceptionConstants.NotAuthorized
                });
            }
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            var hubHttpContext = Context.GetHttpContext();
            var correlationId = hubHttpContext?.GetCorrelationId();

            _logger.LogInformation(
                "Client connected with connectionId {ConnectionId} and correlationId {CorrelationId}",
                Context.ConnectionId,
                correlationId
            );
        }
    }
}
