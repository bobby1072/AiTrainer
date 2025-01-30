using System.Text.Json;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.HttpClient.Extensions;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.CoreClient.Clients.Concrete;

internal class CoreClientUpdateFaissStore: ICoreClient<UpdateFaissStoreInput, FaissStoreResponse>
{
    private readonly ILogger<CoreClientCreateFaissStore> _logger;
    private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;

    public CoreClientUpdateFaissStore(
        ILogger<CoreClientCreateFaissStore> logger,
        IOptionsSnapshot<AiTrainerCoreConfiguration> aiTrainerCoreConfig
    )
    {
        _logger = logger;
        _aiTrainerCoreConfiguration = aiTrainerCoreConfig.Value;
    }

    public async Task<FaissStoreResponse?> TryInvokeAsync(UpdateFaissStoreInput input)
    {
        try
        {
            
            var jsonMetadata = new
            {
                docStore = input.JsonDocStore,
                newDocuments = input.NewDocuments 
            };

            var response = await _aiTrainerCoreConfiguration.BaseEndpoint
                .AppendPathSegment("api")
                .AppendPathSegment("faissrouter")
                .AppendPathSegment("createstore")
                .WithAiTrainerCoreKeyHeader(_aiTrainerCoreConfiguration.ApiKey)
                .PostMultipartAsync(x =>
                {
                    using var indexFileStream = new MemoryStream(input.FileInput);
                    x.AddJson("metadata", jsonMetadata);
                    x.AddFile("file", indexFileStream, "docStore.index");
                })
                .ReceiveJsonAsync<FaissStoreResponse>(_aiTrainerCoreConfiguration)
                .CoreClientExceptionHandling(_logger, nameof(CoreClientUpdateFaissStore));
            
            
            return response;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }
    }
}

// const metadata = JSON.parse(req.body.metadata); // Parse metadata JSON
// const fileBuffer = req.file?.buffer;
// const safeInput = UpdateStoreInputSchema.safeParse({
//     fileInput: fileBuffer,
//     docStore: metadata.docStore,
//     newDocuments: metadata.newDocuments,
// });