using AiTrainer.Web.Domain.Models.Partials;

namespace AiTrainer.Web.Domain.Models
{
    public record FlatFileDocumentPartialCollection
    {
        public required IReadOnlyCollection<FileCollection> FileCollections { get; init; }
        public required IReadOnlyCollection<FileDocumentPartial> FileDocuments { get; init; }
    }
}
