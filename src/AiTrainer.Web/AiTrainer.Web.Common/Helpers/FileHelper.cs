using System.Text;
using UglyToad.PdfPig;

namespace AiTrainer.Web.Common.Helpers;

public static class FileHelper
{

    public static async Task<string> GetTextFromTextFileByteArray(byte[] byteArrayTextFile)
    {
        using var memoryStream = new MemoryStream(byteArrayTextFile);
        using var reader = new StreamReader(memoryStream, Encoding.UTF8);
        var text = await reader.ReadToEndAsync();
        
        return text;
    }

    public static IEnumerable<string> GetTextFromPdfFileByteArray(byte[] byteArrayPdfFile)
    {
        using var memoryStream = new MemoryStream(byteArrayPdfFile);
        using var pdfDoc = PdfDocument.Open(memoryStream);

        foreach (var page in pdfDoc.GetPages())
        {
            yield return page.Text;
        }
    }
}