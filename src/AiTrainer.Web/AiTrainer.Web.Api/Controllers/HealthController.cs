using AiTrainer.Web.Common.Models.ApiModels.Response;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.Domain.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.Api.Controllers
{
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
        public Task<ActionResult<Outcome<ApplicationSettingsConfiguration>>> Health()
        {
            return Task.FromResult(
                (ActionResult<Outcome<ApplicationSettingsConfiguration>>)
                    new Outcome<ApplicationSettingsConfiguration> { Data = _appSettings }
            );
        }
    }
}
