namespace AiTrainer.Web.CoreClient.Client.Abstract
{
    public interface ICoreClient
    {
        Task<IReadOnlyCollection<string>> ChunkDocument(string documentTextToChunk);
        Task<IReadOnlyCollection<string>?> TryChunkDocument(string documentTextToChunk);
    }
}
