using System.Net;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.CoreClient.Service.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers
{
    public class CoreController : BaseController
    {
        private const string _testStory =
            @"In a quiet village nestled between rolling hills, there lived an old clockmaker named Elias. His shop, filled with intricately designed clocks, was a place of wonder for the village children. Each clock ticked in perfect harmony, their rhythmic chimes a familiar melody to all who passed by. Elias had a particular love for an old, dusty grandfather clock that stood in the corner of his shop. Hed found it abandoned in the attic of an ancient mansion and spent years trying to restore it, but it never seemed to tick, no matter how many times he tinkered with it. The clock was special, he could feel it, but its silence left him baffled.
One stormy evening, as thunder rolled through the sky, Elias heard a sound he hadnt expected a faint, deep chime resonated from the grandfather clock. Slowly, its hands began to move, marking midnight. Curiosity piqued, he leaned in closer, only to hear whispers emanating from within. The clock, it seemed, was no ordinary timepiece. It whispered tales of forgotten times, of ancient kings and long-lost worlds. As the storm raged outside, Elias realized the clock wasnt just telling timeit was unlocking the past, revealing secrets no one in the village could ever imagine.
";
        private readonly ICoreClientServiceProvider _coreClientServiceProvider;

        public CoreController(
            ILogger<CoreController> logger,
            ICoreClientServiceProvider coreClientServiceProvider
        )
            : base(logger)
        {
            _coreClientServiceProvider = coreClientServiceProvider;
        }

        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesDefaultResponseType(typeof(string))]
        [HttpGet("hello")]
        public async Task<IActionResult> Hello()
        {
            var testDoc = new DocumentToChunk
            {
                DocumentText = _testStory
            };

            var result = await _coreClientServiceProvider.ExecuteAsync<DocumentToChunk, ChunkedDocument>(testDoc);

            return Ok("Hello from CoreController");
        }
    }
}
