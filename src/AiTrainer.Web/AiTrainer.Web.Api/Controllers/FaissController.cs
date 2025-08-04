using AiTrainer.Web.Api.Attributes;
using AiTrainer.Web.Api.Models;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.ChatGpt.Abstract;
using AiTrainer.Web.Domain.Services.File.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers;

[RequireUserLogin]
public sealed class FaissController : BaseController
{

    public FaissController(
        IHttpDomainServiceActionExecutor actionExecutor
    )
        : base(actionExecutor)
    {
    }

    [HttpPost("Ai/Query/AnalyseDocumentChunkWithQuestion")]
    public async Task<ActionResult<Outcome<string>>> ChatQuery(
        [FromBody] AnalyseDocumentChunkInReferenceToQuestionQueryInput input,
        CancellationToken ct = default
    )
    {
        var currentUser = await GetCurrentUser();

        var result = await _actionExecutor.ExecuteAsync<IChatGptQueryProcessingManager, string>(
            serv => serv.ChatGptQuery(input, currentUser, ct),
            nameof(IChatGptQueryProcessingManager.ChatGptQuery)
        );

        return new Outcome<string> { Data = result };
    }

    [HttpPost("Sync")]
    public async Task<ActionResult<Outcome>> SyncFaissStore(
        [FromBody] SyncFaissStoreHubInput input,
        CancellationToken ct = default
    )
    {
        var currentUser = await GetCurrentUser();

        await _actionExecutor.ExecuteAsync<IFileCollectionFaissSyncProcessingManager>(
            serv =>
                serv.SyncUserFileCollectionFaissStore(currentUser, input.CollectionId, false, ct),
            nameof(IFileCollectionFaissSyncProcessingManager.SyncUserFileCollectionFaissStore)
        );

        return new Outcome();
    }

    [HttpPost("TriggerSync")]
    public async Task<ActionResult<Outcome>> TriggerSyncFaissStore(
        [FromBody] SyncFaissStoreHubInput input,
        CancellationToken ct = default
    )
    {
        var currentUser = await GetCurrentUser();

        await _actionExecutor.ExecuteAsync<IFileCollectionFaissSyncProcessingManager>(
            serv =>
                serv.TriggerSyncUserFileCollectionFaissStore(currentUser, input.CollectionId, false, ct),
            nameof(IFileCollectionFaissSyncProcessingManager.TriggerSyncUserFileCollectionFaissStore)
        );

        return new Outcome();
    }

    [HttpPost("SimilaritySearch")]
    public async Task<Outcome<IReadOnlyCollection<SingleDocumentChunk>>> SimilaritySearchFaissStore(
        [FromBody] SimilaritySearchInput input,
        CancellationToken ct = default
    )
    {
        var currentUser = await GetCurrentUser();

        var result = await _actionExecutor.ExecuteAsync<
            IFileCollectionFaissSimilaritySearchProcessingManager,
            IReadOnlyCollection<SingleDocumentChunk>
        >(
            serv => serv.SimilaritySearch(input, currentUser, ct),
            nameof(IFileCollectionFaissSimilaritySearchProcessingManager.SimilaritySearch)
        );

        return new Outcome<IReadOnlyCollection<SingleDocumentChunk>> { Data = result };
    }
}
