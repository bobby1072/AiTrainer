using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Clients.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace AiTrainer.Web.CoreClient.Tests;

public class CoreClientChunkDocumentTests
{
    private readonly Mock<ILogger<CoreClientChunkDocument>> _mockLogger = new();
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new();
    private AiTrainerCoreConfiguration _aiTrainerCoreConfiguration => new AiTrainerCoreConfiguration
    {
        ApiKey = Guid.NewGuid().ToString(),
        BaseEndpoint = "http://localhost:5000",
        TotalAttempts = 56,
        TimeoutInSeconds = 2,
        DelayBetweenAttemptsInSeconds = 2
    };
}