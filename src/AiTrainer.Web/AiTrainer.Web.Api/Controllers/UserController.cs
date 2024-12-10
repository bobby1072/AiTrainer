using AiTrainer.Web.Api.Models;
using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers
{
    public class UserController : BaseController
    {
        private readonly IApiRequestHttpContextService _httpContextService;
        public UserController(IDomainServiceActionExecutor actionExecutor,
            IApiRequestHttpContextService httpContextService)
            : base(actionExecutor)
        {
            _httpContextService = httpContextService;
        }

        [HttpGet("IssueDeviceToken")]
        public async Task<ActionResult<Outcome<SolicitedDeviceToken>>> IssueDeviceToken()
        {
            var deviceToken = await _actionExecutor.ExecuteAsync<
                IUserProcessingManager,
                SolicitedDeviceToken
            >(service => service.IssueDeviceToken());

            return new Outcome<SolicitedDeviceToken> { Data = deviceToken };
        }
        [HttpPost("ConfirmUser")]
        public async Task<User> ConfirmUser([FromBody]SaveUserInput userToConfirm)
        {
            var confirmedUser = await _actionExecutor.ExecuteAsync<IUserProcessingManager, User>(
                service => service.ConfirmUser(userToConfirm, _httpContextService.DeviceToken)
            );

            return confirmedUser;
        }
    }
}
