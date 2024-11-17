using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models;

namespace AiTrainer.Web.Domain.Services.File.Abstract
{
    public interface IFileDocumentProcessingManager
    {
        Task<FileDocument> UploadFile(FileDocumentSaveFormInput fileDocumentSaveFormInput);
    }
}
