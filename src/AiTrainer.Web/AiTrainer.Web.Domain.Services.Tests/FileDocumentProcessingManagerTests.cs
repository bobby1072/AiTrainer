using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Domain.Services.File.Concrete;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.TestBase.Utils;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;

namespace AiTrainer.Web.Domain.Services.Tests
{
    public class FileDocumentProcessingManagerTests : DomainServiceTestBase
    {
        private readonly Mock<ILogger<FileDocumentProcessingManager>> _mockLogger = new();
        private readonly Mock<IFileDocumentRepository> _mockFileDocumentRepository = new();
        private readonly Mock<IValidator<FileDocument>> _mockValidator = new();
        private readonly Mock<IFileCollectionRepository> _mockFileCollectionRepository = new();
        private readonly Mock<IFileCollectionFaissRepository> _mockFileFaissReposiotory = new();
        private readonly Mock<IFileCollectionFaissSyncBackgroundJobQueue> _jobQueueMock = new();
        private readonly FileDocumentProcessingManager _fileDocumentProcessingManager;
        public FileDocumentProcessingManagerTests()
        {
            _fileDocumentProcessingManager = new FileDocumentProcessingManager(
                _mockLogger.Object,
                _mockFileDocumentRepository.Object,
                _mockFileFaissReposiotory.Object,
                _mockValidator.Object,
                _mockFileCollectionRepository.Object,
                _jobQueueMock.Object,
                MockContextAccessor.Object
            );
            AddAccessTokenToRequestHeaders();
        }

        [Fact]
        public async Task UploadFile_Should_Throw_If_No_Entities_Returned_From_Save()
        {
            //Arrange
            var currentUser = Fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            var mockForm = FormFileUtils.CreateFromFile();
            var fileDocInput = Fixture
                .Build<FileDocumentSaveFormInput>()
                .With(x => x.CollectionId, (Guid?)null)
                .With(x => x.FileToCreate, mockForm)
                .Create();

            _mockValidator
                .Setup(x => x.ValidateAsync(It.IsAny<FileDocument>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockFileDocumentRepository
                .Setup(x => x.Create(It.IsAny<IReadOnlyCollection<FileDocument>>()))
                .ReturnsAsync(new DbSaveResult<FileDocument>());

            //Act
            var act = () => _fileDocumentProcessingManager.UploadFileDocument(fileDocInput, currentUser);

            //Assert
            await act.Should().ThrowAsync<ApiException>().WithMessage("Invalid file document");
        }

        [Fact]
        public async Task DeleteFileDocument_Should_Successfully_Delete_Authorized_Users_Document()
        {
            //Arrange
            var collectionId = Guid.NewGuid();

            var currentUser = Fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();

            var docToDelete = Fixture
                .Build<FileDocument>()
                .With(x => x.UserId, (Guid)currentUser.Id!)
                .With(x => x.CollectionId, collectionId)
                .Create();

            _mockFileDocumentRepository.Setup(x =>
                x.GetOne((Guid)docToDelete.Id!)
            ).ReturnsAsync(new DbGetOneResult<FileDocument>(docToDelete));

            _mockFileFaissReposiotory.Setup(x =>
                x.DeleteDocumentAndStoreAndUnsyncDocuments(docToDelete)
            ).ReturnsAsync(new DbResult(true));

            //Act
            var result = await _fileDocumentProcessingManager.DeleteFileDocument(collectionId, currentUser);

            //Assert
        }
    }
}
