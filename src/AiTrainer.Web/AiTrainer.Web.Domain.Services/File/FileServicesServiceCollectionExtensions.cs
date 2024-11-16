

using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Domain.Services.File.Concrete;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.Domain.Services.File
{
    internal static class FileServicesServiceCollectionExtensions
    {
        public static IServiceCollection AddFileServices(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<IFileCollectionProcessingManager, FileCollectionProcessingManager>()
                .AddTransient<IFileDocumentProcessingManager, FileDocumentProcessingManager>();

            return serviceCollection;
        }
    }
}
