using System.Net;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected IHttpDomainServiceActionExecutor _actionExecutor;
        protected BaseController(IHttpDomainServiceActionExecutor actionExecutor)
        {
            _actionExecutor = actionExecutor;
        }

        protected async Task<User> GetCurrentUser() =>
            await _actionExecutor.ExecuteAsync<IUserProcessingManager, User?>(service => service.TryGetUserFromCache(
               HttpContext.GetAccessToken() 
            ), nameof(IUserProcessingManager.TryGetUserFromCache)) ?? throw new ApiException(ExceptionConstants.Unauthorized, HttpStatusCode.Unauthorized);
    }
}
