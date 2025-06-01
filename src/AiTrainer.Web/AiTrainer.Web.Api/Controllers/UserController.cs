using AiTrainer.Web.Api.Attributes;
using AiTrainer.Web.Api.Models;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers;

[RequireUserLogin]
public sealed class UserController : BaseController
{
    public UserController(IHttpDomainServiceActionExecutor actionExecutor)
        : base(actionExecutor) { }

    [HttpGet]
    public async Task<ActionResult<Outcome<User?>>> GetSelf()
    {
        var self = await GetCurrentUser();

        return new Outcome<User?> { Data = self };
    }
}
