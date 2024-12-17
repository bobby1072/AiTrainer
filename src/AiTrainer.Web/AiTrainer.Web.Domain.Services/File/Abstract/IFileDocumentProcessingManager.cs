using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.Partials;
using AiTrainer.Web.Domain.Services.Abstract;

namespace AiTrainer.Web.Domain.Services.File.Abstract
{
    public interface IFileDocumentProcessingManager : IDomainService
    {
        Task<FileDocument> GetFileDocumentForDownload(Guid documentId);
        Task<Guid> DeleteFileDocument(Guid documentId);
        Task<FileDocumentPartial> UploadFileDocument(
            FileDocumentSaveFormInput fileDocumentSaveFormInput
        );
    }
}
