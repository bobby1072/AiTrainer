using System.Net;
using AiTrainer.Web.Api.SignalR.Models;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Api.SignalR.Hubs
{
    public class AiTrainerHub : Hub
    {
        private readonly IHttpDomainServiceActionExecutor _iHttpDomainService;
        private readonly ILogger<AiTrainerHub> _logger;

        public AiTrainerHub(
            ILogger<AiTrainerHub> logger,
            IHttpDomainServiceActionExecutor iHttpDomainService
        )
        {
            _iHttpDomainService = iHttpDomainService;
            _logger = logger;
        }

        [HubMethodName("SimilaritySearchFaissStore")]
        public async Task SimilaritySearchFaissStore(SimilaritySearchInput input)
        {
            var hubHttpContext = Context.GetHttpContext();
            var correlationId = hubHttpContext?.GetCorrelationId();
            try
            {
                _logger.LogInformation(
                    "Client with connectionId {ConnectionId} and correlationId {CorrelationId} is triggering a similarity search collectionId {CollectionId}",
                    Context.ConnectionId,
                    correlationId,
                    input.CollectionId
                );
                var accessToken = hubHttpContext?.GetAccessTokenFromQuery("access_token")!;

                var currentUser =
                    await _iHttpDomainService.ExecuteAsync<IUserProcessingManager, User?>(
                        userProcessingManager =>
                            userProcessingManager.TryGetUserFromCache(accessToken)
                    ) ?? throw new ApiException("Can't find user", HttpStatusCode.Unauthorized);

                var result = await _iHttpDomainService.ExecuteAsync<
                    IFileCollectionFaissSimilaritySearchProcessingManager,
                    IReadOnlyCollection<SingleDocumentChunk>
                >(serv => serv.SimilaritySearch(input, currentUser));

                await Clients.Caller.SendAsync(
                    "SimilaritySearchFaissSuccess",
                    new SignalRClientEvent<IReadOnlyCollection<SingleDocumentChunk>> { Data = result }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error syncing faiss store for connectionId {ConnectionId}, collectionId {CollectionId} and correlationId {CorrelationId}",
                    Context.ConnectionId,
                    input.CollectionId,
                    correlationId
                );

                await Clients.Caller.SendAsync(
                    "SimilaritySearchFaissError",
                    new SignalRClientEvent
                    {
                        ExceptionMessage =
                            "An error occurred whilst trying to do similarity search",
                    }
                );
            }
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
                    await _iHttpDomainService.ExecuteAsync<IUserProcessingManager, User?>(
                        userProcessingManager =>
                            userProcessingManager.TryGetUserFromCache(accessToken)
                    ) ?? throw new ApiException("Can't find user", HttpStatusCode.Unauthorized);

                await _iHttpDomainService.ExecuteAsync<IFileCollectionFaissSyncProcessingManager>(serv =>
                    serv.SyncUserFileCollectionFaissStore(
                        currentUser,
                        input.CollectionId,
                        CancellationToken.None
                    )
                );

                await Clients.Caller.SendAsync(
                    "SyncFaissStoreSuccess",
                    new SignalRClientEvent<string> { Data = "Successfully faiss synced collection" }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error syncing faiss store for connectionId {ConnectionId}, collectionId {CollectionId} and correlationId {CorrelationId}",
                    Context.ConnectionId,
                    input.CollectionId,
                    correlationId
                );

                await Clients.Caller.SendAsync(
                    "SyncFaissStoreError",
                    new SignalRClientEvent
                    {
                        ExceptionMessage = "An error occurred whilst syncing the file collection",
                    }
                );
            }
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            var hubHttpContext = Context.GetHttpContext();
            var correlationId = hubHttpContext?.GetCorrelationId();
            var accessToken = hubHttpContext?.GetAccessTokenFromQuery("access_token")!;

            _ =
                await _iHttpDomainService.ExecuteAsync<IUserProcessingManager, User>(
                    userProcessingManager => userProcessingManager.SaveAndCacheUser(accessToken)
                ) ?? throw new ApiException("Can't find user", HttpStatusCode.Unauthorized);
            _logger.LogInformation(
                "Client connected with connectionId {ConnectionId} and correlationId {CorrelationId} and accessToken {AccessToken}",
                Context.ConnectionId,
                correlationId,
                accessToken
            );
        }
    }
}
