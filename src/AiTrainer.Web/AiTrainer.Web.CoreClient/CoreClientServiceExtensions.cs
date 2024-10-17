using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.CoreClient
{
    public static class CoreClientServiceExtensions
    {
        public static IServiceCollection AddCoreClient(this IServiceCollection serviceCollection) 
        {

            return serviceCollection;
        }
    }
}