using AiTrainer.Web.Api.Models;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers
{
    public class UserController: BaseController
    {
        public UserController(IDomainServiceActionExecutor actionExecutor): base(actionExecutor) { }

        [HttpGet("IssueDeviceToken")]
        public async Task<ActionResult<Outcome<SolicitedDeviceToken>>> IssueDeviceToken()
        {
            var deviceToken = await _actionExecutor.ExecuteAsync<IUserProcessingManager, SolicitedDeviceToken>(service => service.IssueDeviceToken());

            return new Outcome<SolicitedDeviceToken> { Data = deviceToken };
        }

    }
}
