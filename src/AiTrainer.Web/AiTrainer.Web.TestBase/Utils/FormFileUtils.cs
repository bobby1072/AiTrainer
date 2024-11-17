using Microsoft.AspNetCore.Http;
using System.Text;

namespace AiTrainer.Web.TestBase.Utils
{
    public static class FormFileUtils
    {
        public static IFormFile CreateFormFile(string fileContent, string fileName)
        {
            // Convert the string content to a byte array
            var byteArray = Encoding.UTF8.GetBytes(fileContent);

            // Create a memory stream using the byte array
            var stream = new MemoryStream(byteArray);

            // Create the IFormFile object
            var formFile = new FormFile(stream, 0, stream.Length, "file", $"{fileName}.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            return formFile;
        }
        public static IFormFile CreateFromFile() => CreateFormFile(string.Join("\n", Enumerable.Range(0, 20).Select(x => Faker.Lorem.Paragraph())), Faker.Lorem.GetFirstWord());
    }
}
