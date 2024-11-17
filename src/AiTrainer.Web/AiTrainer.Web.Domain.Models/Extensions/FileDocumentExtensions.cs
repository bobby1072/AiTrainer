using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Models.ApiModels.Request;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace AiTrainer.Web.Domain.Models.Extensions
{
    public static class FileDocumentExtensions
    {
        public static async Task<FileDocument> ToDocumentModel(this FileDocumentSaveFormInput formInput, Guid userId)
        {
            var fileNameAndType = formInput.FileToCreate.GetFileType();
            return new FileDocument
            {
                DateCreated = DateTime.UtcNow,
                FileName = fileNameAndType.FileName,
                FileType = fileNameAndType.FileType,
                CollectionId = formInput.CollectionId,
                UserId = userId,
                FileData = await formInput.FileToCreate.ConvertToByteArrayAsync()
            };
        }

        private static (string FileName, FileTypeEnum FileType) GetFileType(this IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName).ToLower();


            return fileExtension switch
            {
                ".pdf" => (Path.GetFileNameWithoutExtension(file.FileName), FileTypeEnum.Pdf),
                ".txt" => (Path.GetFileNameWithoutExtension(file.FileName), FileTypeEnum.Text),
                _ => ("", FileTypeEnum.Null)
            };
        }
        public static async Task<byte[]> ConvertToByteArrayAsync(this IFormFile file)
        {
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
