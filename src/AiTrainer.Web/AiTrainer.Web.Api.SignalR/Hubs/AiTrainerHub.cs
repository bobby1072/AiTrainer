using AiTrainer.Web.Api.SignalR.Models.Requests;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Net;
using AiTrainer.Web.Api.SignalR.Models;

namespace AiTrainer.Web.Api.SignalR.Hubs
{
    public class AiTrainerHub : Hub
    {
        private readonly IDomainServiceActionExecutor _domainService;
        private readonly ILogger<AiTrainerHub> _logger;
        public AiTrainerHub(
            ILogger<AiTrainerHub> logger,
            IDomainServiceActionExecutor domainService
        )
        {
            _domainService = domainService;
            _logger = logger;
        }
        [HubMethodName("SyncFaissStore")]
        public async Task SyncFaissStore(SyncFaissStoreHubInput input)
        {
            var hubHttpContext = Context.GetHttpContext();
            var correlationId = hubHttpContext?.GetCorrelationId();
            try
            {
                _logger.LogInformation(
                    "Client with connectionId {ConnectionId} and correlationId {CorrelationId} has triggered a faiss sync for collectionId {CollectionId}",
                    Context.ConnectionId,
                    correlationId,
                    input.CollectionId
                );

                var accessToken = hubHttpContext?.GetAccessTokenFromQuery("access_token")!;

                var currentUser =
                    await _domainService.ExecuteAsync<IUserProcessingManager, User?>(userProcessingManager =>
                        userProcessingManager.TryGetUserFromCache(accessToken))
                    ?? throw new ApiException("Can't find user", HttpStatusCode.Unauthorized);

                await _domainService.ExecuteAsync<IFileCollectionFaissSyncProcessingManager>(serv =>
                    serv.SyncUserFileCollectionFaissStore(currentUser, input.CollectionId));

                await Clients.Caller.SendAsync("SyncFaissStoreSuccess",new SignalRClientEvent<string> { Data = "Successfully faiss synced collection"});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing faiss store for connectionId {ConnectionId}, collectionId {CollectionId} and correlationId {CorrelationId}",
                    Context.ConnectionId,
                    input.CollectionId,
                    correlationId
                );
                
                await Clients.Caller.SendAsync("SyncFaissStoreError", new SignalRClientEvent { ExceptionMessage = "An error occurred whilst syncing the file collection" });
            }
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
