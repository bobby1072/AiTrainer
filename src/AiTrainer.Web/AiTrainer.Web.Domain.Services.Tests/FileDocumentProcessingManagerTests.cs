using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models.Extensions;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Domain.Services.File.Concrete;
using AiTrainer.Web.Domain.Services.File.Models;
using AiTrainer.Web.Persistence.Entities;
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
        private readonly Mock<IFileCollectionFaissSyncBackgroundJobQueue> _mockJobQueue = new();
        private readonly FileDocumentProcessingManager _fileDocumentProcessingManager;
        public FileDocumentProcessingManagerTests()
        {
            _fileDocumentProcessingManager = new FileDocumentProcessingManager(
                _mockLogger.Object,
                _mockFileDocumentRepository.Object,
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
        public async Task UploadFile_Should_Successfully_Save_Collection_With_No_Collection_Id_For_Authorized_User()
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
                .Setup(x => x.Create(It
                    .Is<IReadOnlyCollection<FileDocument>>(y => y.FirstOrDefault()!.FileDescription == fileDocInput.FileDescription)))
                .ReturnsAsync(new DbSaveResult<FileDocument>([await fileDocInput.ToDocumentModel((Guid)currentUser.Id!)]));

            //Act
            await _fileDocumentProcessingManager.UploadFileDocument(fileDocInput, currentUser);

            //Assert
            _mockValidator
                .Verify(x => x.ValidateAsync(It.IsAny<FileDocument>(), default), Times.Once);
            _mockFileDocumentRepository
                .Verify(x => x.Create(It
                    .Is<IReadOnlyCollection<FileDocument>>(y =>
                        y.FirstOrDefault()!.FileDescription == fileDocInput.FileDescription)), Times.Once);
        }
        [Fact]
        public async Task UploadFile_Should_Throw_When_Saving_Collection_With_Collection_Id_For_Shared_Member_With_No_Permission()
        {
            //Arrange
            var collectionId = Guid.NewGuid();
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            var parentCollection = _fixture
                .Build<FileCollection>()
                .With(x => x.Id, collectionId)
                .With(x => x.SharedFileCollectionMembers, [
                    _fixture
                        .Build<SharedFileCollectionMember>()
                        .With(x => x.CollectionId, collectionId)
                        .With(x => x.UserId, currentUser.Id)
                        .With(x => x.CanCreateDocuments, false)
                        .Create()
                ])
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.UserId, Guid.NewGuid())
                .Create();
            var mockForm = TestFileUtils.CreateFormFile();
            var fileDocInput = _fixture
                .Build<FileDocumentSaveFormInput>()
                .With(x => x.CollectionId, parentCollection.Id)
                .With(x => x.FileToCreate, mockForm)
                .Create();
            _mockFileCollectionRepository
                .Setup(x => x
                    .GetOne(collectionId, nameof(FileCollectionEntity.SharedFileCollectionMembers)))
                .ReturnsAsync(new DbGetOneResult<FileCollection>(parentCollection));
            _mockValidator
                .Setup(x => x.ValidateAsync(It.IsAny<FileDocument>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockFileDocumentRepository
                .Setup(x => x.Create(It
                    .Is<IReadOnlyCollection<FileDocument>>(y => y.FirstOrDefault()!.FileDescription == fileDocInput.FileDescription)))
                .ReturnsAsync(new DbSaveResult<FileDocument>([await fileDocInput.ToDocumentModel((Guid)currentUser.Id!)]));

            //Act
            var act = () => _fileDocumentProcessingManager.UploadFileDocument(fileDocInput, currentUser);

            //Assert
            var ex = await Assert.ThrowsAsync<ApiException>(act);
            Assert.Equal(ExceptionConstants.Unauthorized, ex.Message);

            _mockValidator
                .Verify(x => x.ValidateAsync(It.IsAny<FileDocument>(), default), Times.Once);
            _mockFileCollectionRepository
                .Verify(x => x
                    .GetOne(collectionId, nameof(FileCollectionEntity.SharedFileCollectionMembers)), Times.Once);
            _mockFileDocumentRepository
                .Verify(x => x.Create(It.IsAny<IReadOnlyCollection<FileDocument>>()), Times.Never);
        }
        [Fact]
        public async Task UploadFile_Should_Successfully_Save_Collection_With_Collection_Id_For_Shared_Member()
        {
            //Arrange
            var collectionId = Guid.NewGuid();
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            var parentCollection = _fixture
                .Build<FileCollection>()
                .With(x => x.Id, collectionId)
                .With(x => x.SharedFileCollectionMembers, [
                    _fixture
                        .Build<SharedFileCollectionMember>()
                        .With(x => x.CollectionId, collectionId)
                        .With(x => x.UserId, currentUser.Id)
                        .With(x => x.CanCreateDocuments, true)
                        .Create()
                ])
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.UserId, Guid.NewGuid())
                .Create();
            var mockForm = TestFileUtils.CreateFormFile();
            var fileDocInput = _fixture
                .Build<FileDocumentSaveFormInput>()
                .With(x => x.CollectionId, parentCollection.Id)
                .With(x => x.FileToCreate, mockForm)
                .Create();
            _mockFileCollectionRepository
                .Setup(x => x
                    .GetOne(collectionId, nameof(FileCollectionEntity.SharedFileCollectionMembers)))
                .ReturnsAsync(new DbGetOneResult<FileCollection>(parentCollection));
            _mockValidator
                .Setup(x => x.ValidateAsync(It.IsAny<FileDocument>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockFileDocumentRepository
                .Setup(x => x.Create(It
                    .Is<IReadOnlyCollection<FileDocument>>(y => y.FirstOrDefault()!.FileDescription == fileDocInput.FileDescription)))
                .ReturnsAsync(new DbSaveResult<FileDocument>([await fileDocInput.ToDocumentModel((Guid)currentUser.Id!)]));

            //Act
            await _fileDocumentProcessingManager.UploadFileDocument(fileDocInput, currentUser);

            //Assert
            _mockValidator
                .Verify(x => x.ValidateAsync(It.IsAny<FileDocument>(), default), Times.Once);
            _mockFileCollectionRepository
                .Verify(x => x
                    .GetOne(collectionId, nameof(FileCollectionEntity.SharedFileCollectionMembers)), Times.Once);
            _mockFileDocumentRepository
                .Verify(x => x.Create(It.IsAny<IReadOnlyCollection<FileDocument>>()), Times.Once);
        }
        [Fact]
        public async Task UploadFile_Should_Successfully_Save_Collection_With_Collection_Id_For_Authorized_User()
        {
            //Arrange
            var collectionId = Guid.NewGuid();
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            var parentCollection = _fixture
                .Build<FileCollection>()
                .With(x => x.Id, collectionId)
                .With(x => x.SharedFileCollectionMembers, [])
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.UserId, currentUser.Id)
                .Create();
            var mockForm = TestFileUtils.CreateFormFile();
            var fileDocInput = _fixture
                .Build<FileDocumentSaveFormInput>()
                .With(x => x.CollectionId, parentCollection.Id)
                .With(x => x.FileToCreate, mockForm)
                .Create();
            _mockFileCollectionRepository
                .Setup(x => x
                    .GetOne(collectionId, nameof(FileCollectionEntity.SharedFileCollectionMembers)))
                .ReturnsAsync(new DbGetOneResult<FileCollection>(parentCollection));
            _mockValidator
                .Setup(x => x.ValidateAsync(It.IsAny<FileDocument>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockFileDocumentRepository
                .Setup(x => x.Create(It
                    .Is<IReadOnlyCollection<FileDocument>>(y => y.FirstOrDefault()!.FileDescription == fileDocInput.FileDescription)))
                .ReturnsAsync(new DbSaveResult<FileDocument>([await fileDocInput.ToDocumentModel((Guid)currentUser.Id!)]));

            //Act
            await _fileDocumentProcessingManager.UploadFileDocument(fileDocInput, currentUser);

            //Assert
            _mockValidator
                .Verify(x => x.ValidateAsync(It.IsAny<FileDocument>(), default), Times.Once);
            _mockFileCollectionRepository
                .Verify(x => x
                    .GetOne(collectionId, nameof(FileCollectionEntity.SharedFileCollectionMembers)), Times.Once);
            _mockFileDocumentRepository
                .Verify(x => x.Create(It.IsAny<IReadOnlyCollection<FileDocument>>()), Times.Once);
        }        
        [Fact]
        public async Task UploadFile_Should_Throw_For_Unauthorized_User()
        {
            //Arrange
            var collectionId = Guid.NewGuid();
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            var parentCollection = _fixture
                .Build<FileCollection>()
                .With(x => x.Id, collectionId)
                .With(x => x.SharedFileCollectionMembers, [])
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.UserId, Guid.NewGuid())
                .Create();
            var mockForm = TestFileUtils.CreateFormFile();
            var fileDocInput = _fixture
                .Build<FileDocumentSaveFormInput>()
                .With(x => x.CollectionId, parentCollection.Id)
                .With(x => x.FileToCreate, mockForm)
                .Create();
            _mockFileCollectionRepository
                .Setup(x => x
                    .GetOne(collectionId, nameof(FileCollectionEntity.SharedFileCollectionMembers)))
                .ReturnsAsync(new DbGetOneResult<FileCollection>(parentCollection));
            _mockValidator
                .Setup(x => x.ValidateAsync(It.IsAny<FileDocument>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockFileDocumentRepository
                .Setup(x => x.Create(It
                    .Is<IReadOnlyCollection<FileDocument>>(y => y.FirstOrDefault()!.FileDescription == fileDocInput.FileDescription)))
                .ReturnsAsync(new DbSaveResult<FileDocument>([await fileDocInput.ToDocumentModel((Guid)currentUser.Id!)]));

            //Act
            var act = () => _fileDocumentProcessingManager.UploadFileDocument(fileDocInput, currentUser);

            //Assert
            var ex = await Assert.ThrowsAsync<ApiException>(act);
            Assert.Equal(ExceptionConstants.Unauthorized, ex.Message);
            
            _mockValidator
                .Verify(x => x.ValidateAsync(It.IsAny<FileDocument>(), default), Times.Once);
            _mockFileCollectionRepository
                .Verify(x => x
                    .GetOne(collectionId, nameof(FileCollectionEntity.SharedFileCollectionMembers)), Times.Once);
            _mockFileDocumentRepository
                .Verify(x => x.Create(It.IsAny<IReadOnlyCollection<FileDocument>>()), Times.Never);
        }
        [Fact]
        public async Task DeleteFileDocument_Should_Throw_If_Shared_Member_No_Permissions()
        {
            //Arrange
            var collectionId = Guid.NewGuid();
            
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            
            var parentCollection = _fixture
                .Build<FileCollection>()
                .With(x => x.Id, collectionId)
                .With(x => x.SharedFileCollectionMembers, [
                    _fixture
                        .Build<SharedFileCollectionMember>()
                        .With(x => x.CollectionId, collectionId)
                        .With(x => x.UserId, currentUser.Id)
                        .With(x => x.CanRemoveDocuments, false)
                        .Create()
                ])
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.UserId, Guid.NewGuid())
                .Create();
            
            var fileDocumentId = Guid.NewGuid();
                
            var docToDelete = _fixture
                .Build<FileDocument>()
                .With(x => x.Id, fileDocumentId)
                .With(x => x.UserId, Guid.NewGuid())
                .With(x => x.CollectionId, collectionId)
                .Create();
            

            _mockFileDocumentRepository.Setup(x =>
                x.GetOne((Guid)docToDelete.Id!)
            ).ReturnsAsync(new DbGetOneResult<FileDocument>(docToDelete));

            _mockFileCollectionRepository
                .Setup(x => x
                    .GetOne(collectionId, nameof(FileCollectionEntity.SharedFileCollectionMembers)))
                .ReturnsAsync(new DbGetOneResult<FileCollection>(parentCollection));

            //Act
            var act = () => _fileDocumentProcessingManager.DeleteFileDocument((Guid)docToDelete.Id!, currentUser);

            //Assert
            var ex = await Assert.ThrowsAsync<ApiException>(act);
            Assert.Equal(ExceptionConstants.Unauthorized, ex.Message);
            
            _mockFileDocumentRepository.Verify(x =>
                x.GetOne((Guid)docToDelete.Id!),
                Times.Once
            );
            _mockFileCollectionRepository
                .Verify(x => x
                    .GetOne(collectionId, nameof(FileCollectionEntity.SharedFileCollectionMembers)), Times.Once);
            
            _mockFileDocumentRepository.Verify(x =>
                x.Delete(It.IsAny<IReadOnlyCollection<FileDocument>>()),
                Times.Never
            );
            _mockJobQueue.Verify(x =>
                x.EnqueueAsync(It.IsAny<FileCollectionFaissRemoveDocumentsBackgroundJob>() , It.IsAny<CancellationToken>()),
                Times.Never
            );
        }
        [Fact]
        public async Task DeleteFileDocument_Should_Successfully_Delete_Document_For_Shared_Member_With_Permissions()
        {
            //Arrange
            var collectionId = Guid.NewGuid();
            
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            
            var parentCollection = _fixture
                .Build<FileCollection>()
                .With(x => x.Id, collectionId)
                .With(x => x.SharedFileCollectionMembers, [
                    _fixture
                        .Build<SharedFileCollectionMember>()
                        .With(x => x.CollectionId, collectionId)
                        .With(x => x.UserId, currentUser.Id)
                        .With(x => x.CanRemoveDocuments, true)
                        .Create()
                ])
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.UserId, Guid.NewGuid())
                .Create();
            
            var fileDocumentId = Guid.NewGuid();
                
            var docToDelete = _fixture
                .Build<FileDocument>()
                .With(x => x.Id, fileDocumentId)
                .With(x => x.UserId, Guid.NewGuid())
                .With(x => x.CollectionId, collectionId)
                .Create();
            

            _mockFileDocumentRepository.Setup(x =>
                x.GetOne((Guid)docToDelete.Id!)
            ).ReturnsAsync(new DbGetOneResult<FileDocument>(docToDelete));

            _mockFileCollectionRepository
                .Setup(x => x
                    .GetOne(collectionId, nameof(FileCollectionEntity.SharedFileCollectionMembers)))
                .ReturnsAsync(new DbGetOneResult<FileCollection>(parentCollection));
            
            
            _mockFileDocumentRepository.Setup(x =>
                x.Delete(It.Is<IReadOnlyCollection<FileDocument>>(x => x.FirstOrDefault() == docToDelete))
            ).ReturnsAsync(new DbDeleteResult<FileDocument>(new List<FileDocument>{docToDelete}));

            //Act
            await _fileDocumentProcessingManager.DeleteFileDocument((Guid)docToDelete.Id!, currentUser);

            //Assert
            _mockFileDocumentRepository.Verify(x =>
                x.GetOne((Guid)docToDelete.Id!),
                Times.Once
            );
            _mockFileCollectionRepository
                .Verify(x => x
                    .GetOne(collectionId, nameof(FileCollectionEntity.SharedFileCollectionMembers)), Times.Once);
            
            _mockFileDocumentRepository.Verify(x =>
                x.Delete(It.Is<IReadOnlyCollection<FileDocument>>(x => x.FirstOrDefault() == docToDelete)),
                Times.Once
            );
            _mockJobQueue.Verify(x =>
                x.EnqueueAsync(It.Is<FileCollectionFaissRemoveDocumentsBackgroundJob>(y => y.CollectionId == collectionId && currentUser.Equals(y.CurrentUser)) , It.IsAny<CancellationToken>()),
                Times.Once
            );
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
            
            var parentCollection = _fixture
                .Build<FileCollection>()
                .With(x => x.Id, collectionId)
                .With(x => x.SharedFileCollectionMembers, [])
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.UserId, currentUser.Id)
                .Create();
            
            var fileDocumentId = Guid.NewGuid();
                
            var docToDelete = _fixture
                .Build<FileDocument>()
                .With(x => x.Id, fileDocumentId)
                .With(x => x.UserId, (Guid)currentUser.Id!)
                .With(x => x.CollectionId, collectionId)
                .Create();
            

            _mockFileDocumentRepository.Setup(x =>
                x.GetOne((Guid)docToDelete.Id!)
            ).ReturnsAsync(new DbGetOneResult<FileDocument>(docToDelete));

            _mockFileCollectionRepository
                .Setup(x => x
                    .GetOne(collectionId, nameof(FileCollectionEntity.SharedFileCollectionMembers)))
                .ReturnsAsync(new DbGetOneResult<FileCollection>(parentCollection));
            
            
            _mockFileDocumentRepository.Setup(x =>
                x.Delete(It.Is<IReadOnlyCollection<FileDocument>>(x => x.FirstOrDefault() == docToDelete))
            ).ReturnsAsync(new DbDeleteResult<FileDocument>(new List<FileDocument>{docToDelete}));

            //Act
            await _fileDocumentProcessingManager.DeleteFileDocument((Guid)docToDelete.Id!, currentUser);

            //Assert
            _mockFileDocumentRepository.Verify(x =>
                x.GetOne((Guid)docToDelete.Id!),
                Times.Once
            );
            _mockFileCollectionRepository
                .Verify(x => x
                    .GetOne(collectionId, nameof(FileCollectionEntity.SharedFileCollectionMembers)), Times.Once);
            
            _mockFileDocumentRepository.Verify(x =>
                x.Delete(It.Is<IReadOnlyCollection<FileDocument>>(x => x.FirstOrDefault() == docToDelete)),
                Times.Once
            );
            _mockJobQueue.Verify(x =>
                x.EnqueueAsync(It.Is<FileCollectionFaissRemoveDocumentsBackgroundJob>(y => y.CollectionId == collectionId && currentUser.Equals(y.CurrentUser)) , It.IsAny<CancellationToken>()),
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

            _mockFileDocumentRepository.Setup(x =>
                x.Delete(new List<FileDocument>{docToDelete})
            ).ReturnsAsync(new DbDeleteResult<FileDocument>(new List<FileDocument>{docToDelete}));

            //Act
            var act = () => _fileDocumentProcessingManager.DeleteFileDocument((Guid)docToDelete.Id!, currentUser);

            //Assert
            var ex = await Assert.ThrowsAsync<ApiException>(act);
            Assert.Equal(ExceptionConstants.Unauthorized, ex.Message);
            
            _mockFileDocumentRepository.Verify(x =>
                    x.GetOne((Guid)docToDelete.Id!),
                Times.Once
            );
            _mockFileDocumentRepository.Verify(x =>
                    x.Delete(It.IsAny<IReadOnlyCollection<FileDocument>>()),
                Times.Never
            );
            _mockJobQueue.Verify(x =>
                    x.EnqueueAsync(It.IsAny<FileCollectionFaissSyncBackgroundJob>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }
    }
}
