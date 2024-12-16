using AiTrainer.Web.Api.Attributes;
using AiTrainer.Web.Common.Models.ApiModels.Response;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers
{
    [Authorize]
    [RequireUserLogin]
    public class UserController : BaseController
    {
        private readonly IApiRequestHttpContextService _apiRequestHttpContextService;

        public UserController(
            IDomainServiceActionExecutor actionExecutor,
            IApiRequestHttpContextService apiRequestHttpContextService
        )
            : base(actionExecutor)
        {
            _apiRequestHttpContextService = apiRequestHttpContextService;
        }

        [HttpGet]
        public async Task<ActionResult<Outcome<User?>>> GetSelf()
        {
            var self = await _actionExecutor.ExecuteAsync<IUserProcessingManager, User?>(service =>
                service.TryGetUserFromCache(_apiRequestHttpContextService.AccessToken)
            );

            return new Outcome<User?> { Data = self };
        }
    }
}
