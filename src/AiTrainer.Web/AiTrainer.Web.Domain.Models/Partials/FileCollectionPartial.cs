namespace AiTrainer.Web.Domain.Models.Partials
{
    public class FileCollectionPartial : FileCollection
    {
        public new IReadOnlyCollection<FileDocumentPartial>? Documents { get; init; }
    }

}