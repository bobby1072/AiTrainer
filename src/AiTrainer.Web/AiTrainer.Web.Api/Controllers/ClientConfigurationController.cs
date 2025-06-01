using AiTrainer.Web.Api.Models;
using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Domain.Models.ApiModels.Response;
using AiTrainer.Web.Domain.Models.Extensions;
using AiTrainer.Web.Domain.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.Api.Controllers
{
    [AllowAnonymous]
    public sealed class ClientConfigurationController : BaseController
    {
        private readonly ClientSettingsConfiguration _clientSettingsConfiguration;

        public ClientConfigurationController(
            IHttpDomainServiceActionExecutor actionExecutor,
            IOptions<ClientSettingsConfiguration> clientSettingsConfiguration
        )
            : base(actionExecutor)
        {
            _clientSettingsConfiguration = clientSettingsConfiguration.Value;
        }

        [HttpGet]
        public Task<ActionResult<Outcome<ClientConfigurationResponse>>> GetConfig()
        {
            return Task.FromResult<ActionResult<Outcome<ClientConfigurationResponse>>>(
                Ok(new Outcome<ClientConfigurationResponse> { Data = _clientSettingsConfiguration.ToClientConfigurationResponse() })
            );
        }
    }
}
