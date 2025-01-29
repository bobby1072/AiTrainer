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
        private readonly ICoreClient<CreateFaissStoreInput, CreateFaissStoreResponse> _coreClient;

        public HealthController(
            IDomainServiceActionExecutor actionExecutor,
            ICoreClient<CreateFaissStoreInput, CreateFaissStoreResponse> coreClient
        )
            : base(actionExecutor)
        {
            _coreClient = coreClient;
        }

        [HttpGet]
        public async Task<ActionResult<Outcome<Domain.Models.AiTrainerHealth>>> Health()
        {
            var res = await _coreClient.TryInvokeAsync(new CreateFaissStoreInput
            {
                Documents = Enumerable
                    .Range(0, 5)
                    .Select(x => Faker.Lorem.Paragraph())
                    .ToArray()
            });
            
            
            var result = await _actionExecutor.ExecuteAsync<IHealthService, Domain.Models.AiTrainerHealth>(service =>
                service.GetHealth()
            );
            
            return new Outcome<Domain.Models.AiTrainerHealth> {
                Data = result
            };
        }
    }
}
