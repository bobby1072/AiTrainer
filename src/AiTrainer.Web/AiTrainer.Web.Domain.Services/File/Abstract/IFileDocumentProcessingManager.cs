using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models.Partials;
using AiTrainer.Web.Domain.Services.Abstract;

namespace AiTrainer.Web.Domain.Services.File.Abstract
{
    public interface IFileDocumentProcessingManager: IDomainService
    {
        Task<FileDocumentPartial> UploadFile(FileDocumentSaveFormInput fileDocumentSaveFormInput);
    }
}
