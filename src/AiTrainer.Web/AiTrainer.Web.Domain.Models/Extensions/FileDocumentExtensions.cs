using AiTrainer.Web.Common.Helpers;
using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models.Partials;
using Microsoft.AspNetCore.Http;

namespace AiTrainer.Web.Domain.Models.Extensions
{
    public static class FileDocumentExtensions
    {
        private static readonly Dictionary<FileTypeEnum, string> _fileTypeToMimeType =
            new() { { FileTypeEnum.Pdf, "application/pdf" }, { FileTypeEnum.Text, "text/plain" } };

        public static string GetMimeType(this FileDocument document)
        {
            return _fileTypeToMimeType[document.FileType];
        }

        public static async Task<FileDocument> ToDocumentModel(
            this FileDocumentSaveFormInput formInput,
            Guid userId
        )
        {
            var fileNameAndType = formInput.FileToCreate.GetFileType();
            return new FileDocument
            {
                DateCreated = DateTime.UtcNow,
                FileName = fileNameAndType.FileName,
                FileType = fileNameAndType.FileType,
                CollectionId = formInput.CollectionId,
                UserId = userId,
                FileDescription = formInput.FileDescription,
                FileData = await formInput.FileToCreate.ConvertToByteArrayAsync(),
            };
        }

        private static (string FileName, FileTypeEnum FileType) GetFileType(this IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            return fileExtension switch
            {
                ".pdf" => (Path.GetFileNameWithoutExtension(file.FileName), FileTypeEnum.Pdf),
                ".txt" => (Path.GetFileNameWithoutExtension(file.FileName), FileTypeEnum.Text),
                _ => ("", FileTypeEnum.Null),
            };
        }

        public static async Task<byte[]> ConvertToByteArrayAsync(this IFormFile file)
        {
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        public static FileDocumentPartial ToPartial(this FileDocument fileDocument)
        {
            return new FileDocumentPartial
            {
                DateCreated = fileDocument.DateCreated,
                FileName = fileDocument.FileName,
                FileType = fileDocument.FileType,
                CollectionId = fileDocument.CollectionId,
                UserId = fileDocument.UserId,
                Id = fileDocument.Id,
            };
        }

        public static async Task<string[]> GetTextFileContent(this FileDocument fileDocument)
        {
            if (fileDocument.FileType == FileTypeEnum.Pdf)
            {
                var pdfText = FileHelper.GetTextFromPdfFileByteArray(fileDocument.FileData);
                
                return pdfText.ToArray();
            }

            else
            {
                var foundText = await FileHelper.GetTextFromTextFileByteArray(fileDocument.FileData);
                
                return [foundText];
            }
        }
    }
}
