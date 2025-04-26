using AiTrainer.Web.Api.Models;
using AiTrainer.Web.Domain.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers
{
    [AllowAnonymous]
    public class HealthController : BaseController
    {
        public HealthController(
            IHttpDomainServiceActionExecutor actionExecutor
        )
            : base(actionExecutor)
        {
        }

        [HttpGet]
        public async Task<ActionResult<Outcome<Domain.Models.AiTrainerHealth>>> Health()
        {   
            var result = await _actionExecutor.ExecuteAsync<IHealthProcessingManager, Domain.Models.AiTrainerHealth>(service =>
                service.GetHealth(), nameof(IHealthProcessingManager.GetHealth)
            );
            
            return new Outcome<Domain.Models.AiTrainerHealth> {
                Data = result
            };
        }
    }
}
