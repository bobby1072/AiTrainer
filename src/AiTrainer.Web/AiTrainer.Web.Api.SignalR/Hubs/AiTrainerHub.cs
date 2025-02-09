using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.Concrete;
using AiTrainer.Web.Domain.Services.User.Abstract;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Net;
using AiTrainer.Web.Api.SignalR.Models.Requests;
using AiTrainer.Web.Domain.Services.File.Abstract;

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

        public async Task SyncFaissStore(SyncFaissStoreHubInput input)
        {
            _logger.LogInformation("Client with connectionId {ConnectionId} has triggered a faiss sync for collectionId {CollectionId}",
                Context.ConnectionId,
                input.CollectionId
            );
            
            var hubHttpContext = Context.GetHttpContext();
            var accessToken = hubHttpContext?.GetAccessTokenFromQuery("access_token")!;
            
            var currentUser =
                await _domainService.ExecuteAsync<IUserProcessingManager, User?>(userProcessingManager => userProcessingManager.TryGetUserFromCache(accessToken))
                ?? throw new ApiException("Can't find user", HttpStatusCode.Unauthorized);
            
            await _domainService.ExecuteAsync<IFileCollectionFaissSyncProcessingManager>(serv =>
                serv.SyncUserFileCollectionFaissStore(currentUser,input.CollectionId));

            await Clients.Caller.SendAsync("Successfully faiss synced collection");
        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            var hubHttpContext = Context.GetHttpContext();
            var correlationId = hubHttpContext?.GetCorrelationId();
            var accessToken = hubHttpContext?.GetAccessTokenFromQuery("access_token")!;
            
            _ =
                await _domainService.ExecuteAsync<IUserProcessingManager, User>(userProcessingManager => userProcessingManager.SaveAndCacheUser(accessToken))
                ?? throw new ApiException("Can't find user", HttpStatusCode.Unauthorized);
            _logger.LogInformation(
                "Client connected with connectionId {ConnectionId} and correlationId {CorrelationId} and accessToken {AccessToken}",
                Context.ConnectionId,
                correlationId,
                accessToken
            );
        }
    }
}
