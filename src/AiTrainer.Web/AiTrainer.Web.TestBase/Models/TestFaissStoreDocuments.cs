using System.Text.Json;

namespace AiTrainer.Web.TestBase.Models;

public record TestFaissStoreDocuments
{
    public required JsonDocument DocStore { get; init; }
    public required byte[] FaissIndex { get; init; }
}