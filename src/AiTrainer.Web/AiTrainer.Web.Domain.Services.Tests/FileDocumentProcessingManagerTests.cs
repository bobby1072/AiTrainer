using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Domain.Services.File.Concrete;
using AiTrainer.Web.Domain.Services.File.Models;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.TestBase;
using AiTrainer.Web.TestBase.Utils;
using AutoFixture;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;

namespace AiTrainer.Web.Domain.Services.Tests
{
    public class FileDocumentProcessingManagerTests : AiTrainerTestBase
    {
        private readonly Mock<ILogger<FileDocumentProcessingManager>> _mockLogger = new();
        private readonly Mock<IFileDocumentRepository> _mockFileDocumentRepository = new();
        private readonly Mock<IValidator<FileDocument>> _mockValidator = new();
        private readonly Mock<IFileCollectionRepository> _mockFileCollectionRepository = new();
        private readonly Mock<IFileCollectionFaissRepository> _mockFileFaissReposiotory = new();
        private readonly Mock<IFileCollectionFaissSyncBackgroundJobQueue> _mockJobQueue = new();
        private readonly FileDocumentProcessingManager _fileDocumentProcessingManager;
        public FileDocumentProcessingManagerTests()
        {
            _fileDocumentProcessingManager = new FileDocumentProcessingManager(
                _mockLogger.Object,
                _mockFileDocumentRepository.Object,
                _mockFileFaissReposiotory.Object,
                _mockValidator.Object,
                _mockFileCollectionRepository.Object,
                _mockJobQueue.Object,
                _mockHttpContextAccessor.Object
            );
        }

        [Fact]
        public async Task UploadFile_Should_Throw_If_No_Entities_Returned_From_Save()
        {
            //Arrange
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            var mockForm = TestFileUtils.CreateFormFile();
            var fileDocInput = _fixture
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
            await Assert.ThrowsAsync<ApiException>(act);
        }

        [Fact]
        public async Task DeleteFileDocument_Should_Successfully_Delete_Authorized_Users_Document()
        {
            //Arrange
            var collectionId = Guid.NewGuid();

            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();

            var docToDelete = _fixture
                .Build<FileDocument>()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.UserId, (Guid)currentUser.Id!)
                .With(x => x.CollectionId, collectionId)
                .Create();

            _mockFileDocumentRepository.Setup(x =>
                x.GetOne((Guid)docToDelete.Id!)
            ).ReturnsAsync(new DbGetOneResult<FileDocument>(docToDelete));

            _mockFileFaissReposiotory.Setup(x =>
                x.DeleteDocumentAndUnsyncDocuments(docToDelete)
            ).ReturnsAsync(new DbResult(true));

            //Act
            var result = await _fileDocumentProcessingManager.DeleteFileDocument((Guid)docToDelete.Id!, currentUser);

            //Assert
            _mockFileDocumentRepository.Verify(x =>
                x.GetOne((Guid)docToDelete.Id!),
                Times.Once
            );
            _mockFileFaissReposiotory.Verify(x =>
                x.DeleteDocumentAndUnsyncDocuments(docToDelete),
                Times.Once
            );
            _mockJobQueue.Verify(x =>
                x.Enqueue(It.Is<FileCollectionFaissSyncBackgroundJob>(y => y.CollectionId == collectionId && currentUser.Equals(y.User))),
                Times.Once
            );
        }
        [Fact]
        public async Task DeleteFileDocument_Should_Throw_For_Unauthorized_Users()
        {
            //Arrange
            var collectionId = Guid.NewGuid();

            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();

            var docToDelete = _fixture
                .Build<FileDocument>()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.UserId, Guid.NewGuid())
                .With(x => x.CollectionId, collectionId)
                .Create();

            _mockFileDocumentRepository.Setup(x =>
                x.GetOne((Guid)docToDelete.Id!)
            ).ReturnsAsync(new DbGetOneResult<FileDocument>(docToDelete));

            _mockFileFaissReposiotory.Setup(x =>
                x.DeleteDocumentAndUnsyncDocuments(docToDelete)
            ).ReturnsAsync(new DbResult(true));

            //Act
            var act = () => _fileDocumentProcessingManager.DeleteFileDocument((Guid)docToDelete.Id!, currentUser);

            //Assert
            await Assert.ThrowsAsync<ApiException>(act);
            
            
            _mockFileDocumentRepository.Verify(x =>
                    x.GetOne((Guid)docToDelete.Id!),
                Times.Once
            );
            _mockFileFaissReposiotory.Verify(x =>
                    x.DeleteDocumentAndUnsyncDocuments(It.IsAny<FileDocument>()),
                Times.Never
            );
            _mockJobQueue.Verify(x =>
                    x.Enqueue(It.IsAny<FileCollectionFaissSyncBackgroundJob>()),
                Times.Never
            );
        }
    }
}
