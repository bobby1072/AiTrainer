using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.TestBase;
using AutoFixture;
using Flurl.Http.Testing;

namespace AiTrainer.Web.CoreClient.Tests
{
    public abstract class CoreClientTestBase: AiTrainerTestBase, IDisposable
    {
        protected readonly Fixture _fixture = new();
        protected readonly HttpTest _httpTest = new();
        protected readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration = new AiTrainerCoreConfiguration
        {
            ApiKey = Guid.NewGuid().ToString(),
            BaseEndpoint = "http://localhost:5000",
            TotalAttempts = 56,
            TimeoutInSeconds = 2,
            DelayBetweenAttemptsInSeconds = 2
        };
        public void Dispose()
        {
            _httpTest.Dispose();
        }
    }
}
