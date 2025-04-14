using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.TestBase;
using AutoFixture;
using Flurl.Http.Testing;

namespace AiTrainer.Web.CoreClient.Tests
{
    public abstract class CoreClientTestBase: AiTrainerTestBase, IDisposable
    {
        protected readonly HttpTest _httpTest = new();
        protected readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration = new()
        {
            ApiKey = Guid.NewGuid().ToString(),
            BaseEndpoint = "http://localhost:5000",
            TotalAttempts = 3,
            TimeoutInSeconds = 2,
            DelayBetweenAttemptsInSeconds = 0
        };
        public void Dispose()
        {
            _httpTest.Dispose();
        }
    }
}
