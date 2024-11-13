using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Persistence.Repositories.Abstract;
namespace AiTrainer.Web.Domain.Services.File.Concrete
{
    public class FileCollectionProcessingManager
        : BaseDomainService,
            IFileCollectionProcessingManager
    {
        private readonly IFileCollectionRepository _repository;
        public FileCollectionProcessingManager(
            IDomainServiceActionExecutor domainServiceActionExecutor,
            IFileCollectionRepository repository
        )
            : base(domainServiceActionExecutor) 
        {
            _repository = repository;
        }
    }
}
