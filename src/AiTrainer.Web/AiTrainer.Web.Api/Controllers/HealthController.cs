using AiTrainer.Web.Common.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.Api.Controllers
{
    public class HealthController: BaseController
    {
        private readonly ApplicationSettingsConfiguration _appSettings;
        public HealthController(IOptions<ApplicationSettingsConfiguration> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        [HttpGet]
        public Task<IActionResult> Health()
        {
            return Task.FromResult((IActionResult)Ok(_appSettings));
        }
    }
}
