using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Services.ChatGpt.Concrete;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.TestBase;
using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AiTrainer.Web.Domain.Services.Tests;

public class ChatGptQueryProcessingManagerTests: AiTrainerTestBase
{
    private readonly Mock<ICoreClient<
        FormattedChatQueryBuilder,
        CoreFormattedChatQueryResponse
    >> _mockChatFormattedQueryClient = new();
    private readonly Mock<IFileCollectionFaissRepository> _mockFileCollectionFaissRepository = new();
    private readonly Mock<IServiceProvider> _mockServiceProvider = new();
    private readonly Mock<IValidator<ChatGptFormattedQueryInput>> _mockChatGptFormattedQueryValidator = new();
    private readonly Mock<IValidator<AnalyseChunkInReferenceToQuestionQueryInput>> _analyseChunkInReferenceToQuestionValidator = new();

    private readonly ChatGptQueryProcessingManager _service;

    public ChatGptQueryProcessingManagerTests()
    {
        SetUpBasicHttpContext();
        
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<AnalyseChunkInReferenceToQuestionQueryInput>)))
            .Returns(_analyseChunkInReferenceToQuestionValidator.Object);
        
        _service = new ChatGptQueryProcessingManager(
            _mockChatFormattedQueryClient.Object,
            _mockChatGptFormattedQueryValidator.Object,
            _mockFileCollectionFaissRepository.Object,
            new NullLogger<ChatGptQueryProcessingManager>(),
            _mockServiceProvider.Object,
            _mockHttpContextAccessor.Object
        );
    }
}