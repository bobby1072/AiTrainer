namespace AiTrainer.Web.Domain.Models.Partials
{
    public record FileCollectionPartial : FileCollection
    {
        public new IReadOnlyCollection<FileDocumentPartial>? Documents { get; init; }
    }

}