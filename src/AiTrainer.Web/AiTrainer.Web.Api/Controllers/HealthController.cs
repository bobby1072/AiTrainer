using AiTrainer.Web.Common.Models.ApiModels.Response;
using AiTrainer.Web.Domain.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers
{
    [AllowAnonymous]
    public class HealthController : BaseController
    {
        
        public HealthController(
            IDomainServiceActionExecutor actionExecutor
        )
            : base(actionExecutor) { }

        [HttpGet]
        public async Task<ActionResult<Outcome<AiTrainerHealth>>> Health()
        {
            var result = await _actionExecutor.ExecuteAsync<IHealthService, AiTrainerHealth>(service =>
                service.GetHealth()
            );
            
            return new Outcome<AiTrainerHealth> {
                Data = result
            };
        }
    }
}
