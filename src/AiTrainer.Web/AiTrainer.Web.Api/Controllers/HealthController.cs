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
        private readonly ApplicationSettingsConfiguration _appSettings;

        public HealthController(
            IOptions<ApplicationSettingsConfiguration> appSettings,
            IDomainServiceActionExecutor actionExecutor
        )
            : base(actionExecutor)
        {
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public Task<ActionResult<Outcome<HealthResponse>>> Health()
        {
            return Task.FromResult(
                (ActionResult<Outcome<HealthResponse>>)
                    new Outcome<HealthResponse> { Data = new HealthResponse
                    {
                        Name = _appSettings.Name,
                        ReleaseVersion = _appSettings.ReleaseVersion
                    } }
            );
        }
    }
}
