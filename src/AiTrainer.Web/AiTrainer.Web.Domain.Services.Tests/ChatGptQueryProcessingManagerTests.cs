using System.Text.Json;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Services.ChatGpt.Concrete;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.TestBase;
using AiTrainer.Web.TestBase.Utils;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AiTrainer.Web.Domain.Services.Tests;

public sealed class ChatGptQueryProcessingManagerTests: AiTrainerTestBase
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
    [Fact]
    public async Task ChatGptFaissQuery_Should_Correctly_Build_AnalyseChunkInReferenceToQuestionQuery_FormattedQuery()
    {
        //Arrange
        var collectionId = Guid.NewGuid();
        var currentUser = _fixture
            .Build<Models.User>()
            .With(u => u.Id, Guid.NewGuid())
            .Create();
        
        var testFaiss = await TestFaissUtils.GetTestFaissStoreAsync();
        
        var existingFaissStore = new FileCollectionFaiss
        {
            Id = 1,
            CollectionId = collectionId,
            FaissIndex = testFaiss.FaissIndex,
            FaissJson = testFaiss.DocStore,
            UserId = (Guid)currentUser.Id!,
            DateCreated = DateTime.UtcNow,
            DateModified = DateTime.UtcNow,
        };
        var singleChunkToUse = existingFaissStore.SingleDocuments.Value.FirstOrDefault();
        var innerChatQueryStartingInput = new AnalyseChunkInReferenceToQuestionQueryInput
        {
            Question = "What's my salary",
            ChunkId = (Guid)singleChunkToUse?.Id!
        };
        var chatQueryInput = new ChatGptFormattedQueryInput
        {
            CollectionId = collectionId,
            InputJson = JsonDocument.Parse(JsonSerializer.Serialize(innerChatQueryStartingInput)),
            DefinedQueryFormatsEnum = 1
        };

        _mockChatGptFormattedQueryValidator
            .Setup(x => x.ValidateAsync(chatQueryInput, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _analyseChunkInReferenceToQuestionValidator
            .Setup(x => 
                x.ValidateAsync(
                    It.Is<AnalyseChunkInReferenceToQuestionQueryInput>(y => 
                        y.Question == innerChatQueryStartingInput.Question && y.ChunkId == innerChatQueryStartingInput.ChunkId),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mockFileCollectionFaissRepository
            .Setup(x => x.ByUserAndCollectionId((Guid)currentUser.Id!, collectionId))
            .ReturnsAsync(new DbGetOneResult<FileCollectionFaiss>(existingFaissStore));
        _mockChatFormattedQueryClient
            .Setup(x => x.TryInvokeAsync(It.Is<FormattedChatQueryBuilder>(
                    z => 
                        z.HumanMessage == innerChatQueryStartingInput.Question && 
                        z.QueryParameters.Any(y => y.Key == "textChunk" && y.Value == singleChunkToUse.PageContent) &&
                        z.SystemMessage == "You need to analyse questions in reference to this section of text: {textChunk}"
                ),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CoreFormattedChatQueryResponse {Content = "45000 every second"});

        //Act
        var result = await _service.ChatGptFaissQuery(chatQueryInput, currentUser);
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal("45000 every second", result);
        
        _mockChatGptFormattedQueryValidator
            .Verify(x => x.ValidateAsync(chatQueryInput, It.IsAny<CancellationToken>()), Times.Once);
        _analyseChunkInReferenceToQuestionValidator
            .Verify(x =>
                x.ValidateAsync(
                    It.Is<AnalyseChunkInReferenceToQuestionQueryInput>(y =>
                        y.Question == innerChatQueryStartingInput.Question &&
                        y.ChunkId == innerChatQueryStartingInput.ChunkId),
                    It.IsAny<CancellationToken>()), Times.Once);
        _mockFileCollectionFaissRepository
            .Verify(x => x.ByUserAndCollectionId((Guid)currentUser.Id!, collectionId), Times.Once);

        _mockChatFormattedQueryClient
            .Verify(x => x.TryInvokeAsync(It.Is<FormattedChatQueryBuilder>(
                    x => 
                            x.HumanMessage == innerChatQueryStartingInput.Question && 
                            x.QueryParameters.Any(y => y.Key == "textChunk" && y.Value == singleChunkToUse.PageContent) &&
                            x.SystemMessage == "You need to analyse questions in reference to this section of text: {textChunk}"
                    ),
                It.IsAny<CancellationToken>()), Times.Once);
    }
}