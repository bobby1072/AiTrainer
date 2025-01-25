using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UglyToad.PdfPig;

namespace AiTrainer.Web.Domain.Models.Helpers;

public static class FileDocumentMetaDataHelper{
    public static async Task<FileDocumentMetaData> GetFromFormFile(IFormFile formFile, Guid documentId){
        if(formFile.ContentType == "application/pdf"){
            await using var memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            using var pdfDoc = PdfDocument.Open(memoryStream);

            return new FileDocumentMetaData{
                DocumentId = documentId,
                Author = pdfDoc.Information.Author,
                CreationDate = pdfDoc.Information.CreationDate,
                Creator = pdfDoc.Information.Creator,
                Keywords = pdfDoc.Information.Keywords,
                ModifiedDate = pdfDoc.Information.ModifiedDate,
                Producer = pdfDoc.Information.Producer,
                ExtraData = GetExtraData(pdfDoc),
                IsEncrypted = pdfDoc.IsEncrypted,
                NumberOfPages = pdfDoc.NumberOfPages,
                Title = pdfDoc.Information.Title
            };
        }
        return new FileDocumentMetaData{ DocumentId = documentId };
    }


    private static Dictionary<string, object> GetExtraData(PdfDocument pdfDoc){
        var extraData = new Dictionary<string, object>();
        if(pdfDoc.Information.DocumentInformationDictionary is not null){
            foreach(var data in pdfDoc.Information.DocumentInformationDictionary.Data){
                extraData.Add(data.Key, data.Value);
            }
        }
        return extraData;
    }
}