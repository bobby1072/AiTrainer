using System.Runtime.Serialization;
using System.Text.Json;
using AiTrainer.Web.Common;

namespace AiTrainer.Web.Domain.Models.Helpers;

public static class FaissHelper{
    
    public static IReadOnlyCollection<SingleDocumentChunk> GetDocumentChunksFromFaissDocStore(JsonDocument jsonDoc)
    {
        try
        {
            var filesFromDocAwayFromMap = jsonDoc.RootElement.EnumerateArray().First().EnumerateArray();

            var listOfDocChunks = new List<SingleDocumentChunk>();
            foreach (var chunk in filesFromDocAwayFromMap)
            {
                var enumedChunk = chunk.EnumerateArray();
                var chunkId = Guid.Parse(enumedChunk.First().ToString());

                var innerContent =
                    JsonSerializer
                        .Deserialize<SingleInnerFaissJsonDocFileContent>(enumedChunk.Skip(1).First().ToString(), ApiConstants.DefaultCamelCaseSerializerOptions)
                    ?? throw new SerializationException("Failed to deserialise file content from single doc chunk");

                if (innerContent.Metadata?.TryGetValue(nameof(SingleDocumentChunk.FileDocumentId), out var fileDocId) ==
                    true)
                {
                    listOfDocChunks.Add(new SingleDocumentChunk
                    {
                        PageContent = innerContent.PageContent,
                        Metadata = innerContent.Metadata,
                        FileDocumentId = Guid.Parse(fileDocId),
                        Id = chunkId
                    });
                }
            }


            return listOfDocChunks;
        }
        catch
        {
            return [];
        }
    }
}
