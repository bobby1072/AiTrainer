using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Tokens;

namespace AiTrainer.Web.Domain.Models.Helpers;

public static class FileDocumentMetaDataHelper{
    private static readonly PropertyInfo[] _metaDataPropertyInfo = typeof(FileDocumentMetaData).GetProperties();
    public static async Task<FileDocumentMetaData> GetFromFormFile(IFormFile formFile, Guid documentId){
        if (formFile.ContentType != "application/pdf") return new FileDocumentMetaData { DocumentId = documentId };
        await using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        using var pdfDoc = PdfDocument.Open(memoryStream);

        var metaData  = new FileDocumentMetaData{
            DocumentId = documentId,
            Author = pdfDoc.Information.Author,
            CreationDate = pdfDoc.Information.CreationDate,
            Creator = pdfDoc.Information.Creator,
            Keywords = pdfDoc.Information.Keywords,
            ModifiedDate = pdfDoc.Information.ModifiedDate,
            Producer = pdfDoc.Information.Producer,
            IsEncrypted = pdfDoc.IsEncrypted,
            NumberOfPages = pdfDoc.NumberOfPages,
            Title = pdfDoc.Information.Title
        };
        
        metaData.ExtraData = GetExtraData(metaData, pdfDoc.Information.DocumentInformationDictionary?.Data);
            
        return metaData; 
    }


    private static Dictionary<string, string?>? GetExtraData(FileDocumentMetaData metaData,IReadOnlyDictionary<string, IToken?>? pdfDoc){
        var extraData = new Dictionary<string, string?>();
        if(pdfDoc is not null){
            foreach(var data in pdfDoc){
                if (_metaDataPropertyInfo.Any(x => x.Name == data.Key || x.Name == "ModDate"))
                {
                    continue;
                }
                extraData.Add(data.Key, data.Value?.ToString());
            }
        }
        return extraData.Count > 0 ? extraData : null;
    }
}