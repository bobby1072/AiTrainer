using System.Text;
using System.Text.Json;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.File.Concrete;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace AiTrainer.Web.Domain.Services.Tests;

public class FileCollectionFaissSimilaritySearchProcessingManagerTests: DomainServiceTestBase
{
    private readonly Mock<ICoreClient<CoreSimilaritySearchInput, SimilaritySearchCoreResponse>> _mockSimSearchClient = new();
    private readonly Mock<ILogger<FileCollectionFaissSimilaritySearchProcessingManager>> _mockLogger = new();
    private readonly Mock<IFileCollectionFaissRepository> _mockFileColFaissRepo = new();
    private readonly Mock<IValidator<SimilaritySearchInput>> _mockValidator = new();
    private readonly FileCollectionFaissSimilaritySearchProcessingManager _service;

    public FileCollectionFaissSimilaritySearchProcessingManagerTests()
    {
        _service = new FileCollectionFaissSimilaritySearchProcessingManager(
            _mockSimSearchClient.Object,
            _mockLogger.Object,
            _mockFileColFaissRepo.Object,
            _mockValidator.Object,
            _mockHttpContextAccessor.Object
        );
        AddAccessTokenToRequestHeaders();
    }
    [Fact]
    public async Task SimilaritySearch_Should_Throw_If_Someone_Elses_Faiss_Store()
    {
        //Arrange
        var currentUser = _fixture.Build<Domain.Models.User>()
            .With(x => x.Id, Guid.NewGuid())
            .Create();
        var stringJson = JsonSerializer.Serialize(new {Text = Faker.Lorem.Paragraph()});
        await using var memStream = new MemoryStream(Encoding.UTF8.GetBytes(stringJson));
        var fileCollectionFaiss = _fixture.Build<FileCollectionFaiss>()
            .With(x => x.FaissJson, await JsonDocument.ParseAsync(memStream))
            .With(x => x.UserId, Guid.NewGuid())
            .Create();

        var simSearchInput = _fixture.Create<SimilaritySearchInput>();
        _mockValidator.Setup(v => v.ValidateAsync(simSearchInput, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockFileColFaissRepo.Setup(x => x.ByUserAndCollectionId((Guid)currentUser.Id!, simSearchInput.CollectionId))
            .ReturnsAsync(new DbGetOneResult<FileCollectionFaiss>(fileCollectionFaiss));
        
        //Act
        var act = () => _service.SimilaritySearch(simSearchInput, currentUser);
        
        //Assert
        await Assert.ThrowsAsync<ApiException>(act);
        
        _mockValidator.Verify(v => v.ValidateAsync(simSearchInput, It.IsAny<CancellationToken>()), Times.Once);
        _mockFileColFaissRepo.Verify(x => x.ByUserAndCollectionId((Guid)currentUser.Id!, simSearchInput.CollectionId),
            Times.Once);
        _mockSimSearchClient.Verify(x => x.TryInvokeAsync(It.IsAny<CoreSimilaritySearchInput>(), It.IsAny<CancellationToken>()), Times.Never);
        
    }
    
}