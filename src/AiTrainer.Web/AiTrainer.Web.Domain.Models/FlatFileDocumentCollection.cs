using AiTrainer.Web.Domain.Models.Partials;

namespace AiTrainer.Web.Domain.Models
{
    public record  FlatFileDocumentCollection
    {
        public required IReadOnlyCollection<FileCollection> FileCollections { get; init; }
        public required IReadOnlyCollection<FileDocument> FileDocuments { get; init; }
    }
    public record FlatFileDocumentPartialCollection
    {
        public required IReadOnlyCollection<FileCollection> FileCollections { get; init; }
        public required IReadOnlyCollection<FileDocumentPartial> FileDocuments { get; init; }
    }
}
