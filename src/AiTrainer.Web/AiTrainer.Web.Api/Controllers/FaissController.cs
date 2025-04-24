using AiTrainer.Web.Api.Attributes;
using AiTrainer.Web.Api.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.ChatGpt.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers;

[RequireUserLogin]
public class FaissController: BaseController
{
    public FaissController(
        IHttpDomainServiceActionExecutor actionExecutor
    )
        : base(actionExecutor)
    {
    }

    [HttpPost("Chat/Query")]
    public async Task<ActionResult<Outcome<string>>> ChatQuery([FromBody] ChatGptFormattedQueryInput input, CancellationToken ct = default)
    {
        var currentUser = await GetCurrentUser();

        var result = await _actionExecutor
            .ExecuteAsync<IChatGptQueryProcessingManager, string>(serv => serv.ChatGptFaissQuery(input, currentUser, ct));
        
        return new Outcome<string>
        {
            Data = result
        };
    }
}