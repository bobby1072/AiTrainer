
namespace AiTrainer.Web.Domain.Models.ApiModels.Request;

public abstract record SharedFileCollectionSingleMemberSaveInput
{
    public bool CanViewDocuments { get; init; }
    public bool CanDownloadDocuments { get; init; }
    public bool CanCreateDocuments { get; init; }
    public bool CanRemoveDocuments { get; init; }
    public bool CanSimilaritySearch { get; init; }
}