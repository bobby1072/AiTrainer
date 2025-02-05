using AiTrainer.Web.Common.Models.ApiModels.Response;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers
{
    [AllowAnonymous]
    public class HealthController : BaseController
    {
        private readonly ICoreClient<CreateFaissStoreInput, FaissStoreResponse> _createStoreClient;
        private readonly ICoreClient<SimilaritySearchInput, FaissStoreResponse> _simSearchClient;

        private static string[] Statements { get; } =
        {
            "Energy cannot be created or destroyed, only transformed.",
            "The speed of light in a vacuum is approximately 299,792,458 meters per second.",
            "DNA carries genetic information in all living organisms.",
            "Water is composed of two hydrogen atoms and one oxygen atom.",
            "Newton's third law states that for every action, there is an equal and opposite reaction."
        };
        public HealthController(
            IDomainServiceActionExecutor actionExecutor,
            ICoreClient<CreateFaissStoreInput, FaissStoreResponse> createStoreClient,
            ICoreClient<SimilaritySearchInput, FaissStoreResponse> simSearchClient
        )
            : base(actionExecutor)
        {
            _createStoreClient = createStoreClient;
            _simSearchClient = simSearchClient;
        }

        [HttpGet]
        public async Task<ActionResult<Outcome<Domain.Models.AiTrainerHealth>>> Health()
        {   
            var result = await _actionExecutor.ExecuteAsync<IHealthService, Domain.Models.AiTrainerHealth>(service =>
                service.GetHealth()
            );
            
            return new Outcome<Domain.Models.AiTrainerHealth> {
                Data = result
            };
        }
    }
}
