using Microsoft.AspNetCore.Http;
using System.Text;
using AiTrainer.Web.Domain.Models.Extensions;

namespace AiTrainer.Web.TestBase.Utils
{
    public static class TestFileUtils
    {
        public static IFormFile CreateFormFile(string fileContent, string fileName)
        {
            var byteArray = Encoding.UTF8.GetBytes(fileContent);

            var stream = new MemoryStream(byteArray);

            var formFile = new FormFile(stream, 0, stream.Length, "file", $"{fileName}.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain",
            };

            return formFile;
        }
        public static IFormFile CreateFormFile() =>
            CreateFormFile(
                string.Join("\n", Enumerable.Range(0, 20).Select(x => Faker.Lorem.Paragraph())),
                Faker.Lorem.GetFirstWord()
            );
        public static Task<byte[]> CreateFileBytes()
            => CreateFormFile().ConvertToByteArrayAsync();
        
        public static Task<byte[]> CreateFileBytes(string fileContent, string fileName)
            => CreateFormFile(fileContent, fileName).ConvertToByteArrayAsync();
    }
}
