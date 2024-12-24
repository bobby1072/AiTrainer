using AiTrainer.Web.Common.Models.ApiModels.Response;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.Domain.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.Api.Controllers
{
    [AllowAnonymous]
    public class ClientConfigurationController : BaseController
    {
        private readonly ClientSettingsConfiguration _clientSettingsConfiguration;

        public ClientConfigurationController(
            IDomainServiceActionExecutor actionExecutor,
            IOptionsSnapshot<ClientSettingsConfiguration> clientSettingsConfiguration
        )
            : base(actionExecutor)
        {
            _clientSettingsConfiguration = clientSettingsConfiguration.Value;
        }

        [HttpGet]
        public Task<ActionResult<Outcome<ClientSettingsConfiguration>>> GetConfig()
        {
            return Task.FromResult<ActionResult<Outcome<ClientSettingsConfiguration>>>(
                Ok(new Outcome<ClientSettingsConfiguration> { Data = _clientSettingsConfiguration })
            );
        }
    }
}
