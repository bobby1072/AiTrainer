using AiTrainer.Web.Common.Attributes;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Common.Models.ApiModels.Response;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers
{
    [RequireUserLogin]
    public class UserController : BaseController
    {
        public UserController(IDomainServiceActionExecutor actionExecutor)
            : base(actionExecutor) { }

        [HttpGet]
        public async Task<ActionResult<Outcome<User?>>> GetSelf()
        {
            var self = await _actionExecutor.ExecuteAsync<IUserProcessingManager, User?>(service =>
                service.TryGetUserFromCache(HttpContext.GetAccessToken())
            );

            return new Outcome<User?> { Data = self };
        }
    }
}
