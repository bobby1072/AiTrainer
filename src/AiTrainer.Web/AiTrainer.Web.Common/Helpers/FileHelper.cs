using System.Text;
using Microsoft.AspNetCore.Http;
using UglyToad.PdfPig;

namespace AiTrainer.Web.Common.Helpers;

public static class FileHelper
{
    public static async Task<IReadOnlyCollection<string>> GetTextFromPdfFile(IFormFile formFile)
    {
        await using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);
        using var pdfDoc = PdfDocument.Open(memoryStream);
        var  pageStringList = new List<string>();
        
        foreach (var page in pdfDoc.GetPages())
        {
            if (!string.IsNullOrEmpty(page.Text))
            {
                pageStringList.Add(page.Text);
            }
        }
        
        return pageStringList;
    }
    public static async Task<string> GetTextFromTextFile(byte[] byteArrayTextFile)
    {
        await using var memoryStream = new MemoryStream(byteArrayTextFile);
        using var reader = new StreamReader(memoryStream, Encoding.UTF8);
        var text = await reader.ReadToEndAsync();
        
        return text;
    }

    public static async Task<IReadOnlyCollection<string>> GetTextFromPdfFile(byte[] byteArrayPdfFile)
    {
        await using var memoryStream = new MemoryStream(byteArrayPdfFile);
        using var pdfDoc = PdfDocument.Open(memoryStream);
        var  pageStringList = new List<string>();
        
        foreach (var page in pdfDoc.GetPages())
        {
            if (!string.IsNullOrEmpty(page.Text))
            {
                pageStringList.Add(page.Text);
            }
        }
        
        return pageStringList;
    }
}