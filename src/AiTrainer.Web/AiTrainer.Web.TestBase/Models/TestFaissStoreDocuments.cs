using System.Text.Json;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.Helpers;

namespace AiTrainer.Web.TestBase.Models;

public record TestFaissStoreDocuments
{
    public required JsonDocument DocStore { get; init; }
    public required byte[] FaissIndex { get; init; }

    public IReadOnlyCollection<SingleDocumentChunk> SingleDocuments =>
        FaissHelper.GetDocumentChunksFromFaissDocStore(DocStore);
}