using System.Runtime.Serialization;
using System.Text.Json;

namespace AiTrainer.Web.Domain.Models.Helpers;

public static class FaissHelper{
    
    public static IReadOnlyCollection<SingleDocumentChunk> GetDocumentChunksFromFaissDocStore(JsonDocument jsonDoc, Guid fileDocumentId)
    {
        var filesFromDocAwayFromMap = jsonDoc.RootElement.EnumerateArray().First().EnumerateArray();

        var listOfDocChunks = new List<SingleDocumentChunk>();
        foreach (var chunk in filesFromDocAwayFromMap)
        {
            var enumedChunk = chunk.EnumerateArray();
            var chunkId = Guid.Parse(enumedChunk.First().ToString());
            
            var innerContent = 
                JsonSerializer
                    .Deserialize<SingleInnerFaissJsonDocFileContent>(enumedChunk.Skip(1).First().ToString())
                 ?? throw new SerializationException("Failed to deserialise file content from single doc chunk");
            
            listOfDocChunks.Add(new SingleDocumentChunk
            {
                PageContent = innerContent.PageContent,
                MetaData = innerContent.Metadata,
                FileDocumentId = fileDocumentId,
                Id = chunkId
            });
        }
        
        
        return listOfDocChunks;
    }
}