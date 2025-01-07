using AiTrainer.Web.Domain.Services.Abstract;
using Microsoft.AspNetCore.SignalR;

namespace AiTrainer.Web.Api.Hubs
{
    public class AiTrainerHub : Hub
    {
        public const string ConnectionIdUserIdCacheKeyPrefix = "userId-";
        private readonly ICachingService _cachingService;
        private readonly IDomainServiceActionExecutor _domainServiceActionExecutor;
        public AiTrainerHub(ICachingService cachingService,
            IDomainServiceActionExecutor domainServiceActionExecutor)
        {
            _cachingService = cachingService;
            _domainServiceActionExecutor = domainServiceActionExecutor;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            
            
        }
    }
}
