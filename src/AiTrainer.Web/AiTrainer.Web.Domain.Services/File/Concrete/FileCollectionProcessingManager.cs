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
            IFileCollectionRepository repository,
            IDomainServiceActionExecutor domainServiceActionExecutor,
            IApiRequestHttpContextService apiRequestService
        )
            : base(domainServiceActionExecutor, apiRequestService) 
        {
            _repository = repository;
        }
    }
}
