using AiTrainer.Web.Domain.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected IDomainServiceActionExecutor _actionExecutor;
        protected BaseController(IDomainServiceActionExecutor actionExecutor)
        {
            _actionExecutor = actionExecutor;
        }
    }
}
