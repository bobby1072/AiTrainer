using System.Net;
using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.TestBase;
using AiTrainer.Web.TestBase.Helpers;

namespace AiTrainer.Web.CoreClient.Tests
{
    public abstract class CoreClientTestBase: AiTrainerTestBase
    {
        protected readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration = new()
        {
            ApiKey = Guid.NewGuid().ToString(),
            BaseEndpoint = "http://localhost:5000",
            TotalAttempts = 3,
            TimeoutInSeconds = 2,
            DelayBetweenAttemptsInSeconds = 0
        };

        protected TestHttpClient CreateDefaultCoreClientHttpClient<T>(HttpStatusCode statusCode, T responseData, string expectedUrl) where T: class
        {
            var handler = new StaticJsonHandler<T>(responseData, statusCode, ApiConstants.DefaultCamelCaseSerializerOptions);
            var httpClient = new TestHttpClient(handler, expectedUrl);
            httpClient.Timeout = TimeSpan.FromSeconds(2);
            
            return httpClient;
        }
    }
}
