using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers
{
    public class CoreController : BaseController
    {
        public CoreController(ILogger<CoreController> logger)
            : base(logger) { }

        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesDefaultResponseType(typeof(string))]
        [HttpGet("core/hello")]
        public IActionResult Hello()
        {
            return Ok("Hello from CoreController");
        }
    }
}
