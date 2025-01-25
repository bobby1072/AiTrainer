using Microsoft.AspNetCore.Http;
using System.Text;

namespace AiTrainer.Web.TestBase.Utils
{
    public static class FormFileUtils
    {
        public static IFormFile CreateFormFile(string fileContent, string fileName)
        {
            var byteArray = Encoding.UTF8.GetBytes(fileContent);

            using var stream = new MemoryStream(byteArray);

            var formFile = new FormFile(stream, 0, stream.Length, "file", $"{fileName}.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain",
            };

            return formFile;
        }

        public static IFormFile CreateFromFile() =>
            CreateFormFile(
                string.Join("\n", Enumerable.Range(0, 20).Select(x => Faker.Lorem.Paragraph())),
                Faker.Lorem.GetFirstWord()
            );
    }
}
