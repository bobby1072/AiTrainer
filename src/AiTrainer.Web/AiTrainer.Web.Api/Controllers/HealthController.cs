using AiTrainer.Web.Api.Models;
using AiTrainer.Web.Common.Models.ApiModels.Response;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.Domain.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.Api.Controllers
{
    [AllowAnonymous]
    public class HealthController : BaseController
    {
        
        public HealthController(
            IDomainServiceActionExecutor actionExecutor
        )
            : base(actionExecutor) { }

        // [HttpGet]
        // public async Task<ActionResult<Outcome<HealthResponse>>> Health()
        // {
        //     return new Outcome<HealthResponse>
        //     {
        //         Data = new HealthResponse
        //         {
        //             Name = _appSettings.Name,
        //             ReleaseVersion = _appSettings.ReleaseVersion,
        //         },
        //     };
        // }
    }
}
