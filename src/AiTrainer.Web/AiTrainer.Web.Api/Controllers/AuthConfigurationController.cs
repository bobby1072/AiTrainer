using AiTrainer.Web.Api.Models;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.Domain.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers
{
    [AllowAnonymous]
    public class ClientConfigurationController : BaseController
    {
        private readonly ClientSettingsConfiguration _clientSettingsConfiguration;

        public ClientConfigurationController(
            IDomainServiceActionExecutor actionExecutor,
            ClientSettingsConfiguration clientSettingsConfiguration
        )
            : base(actionExecutor)
        {
            _clientSettingsConfiguration = clientSettingsConfiguration;
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
