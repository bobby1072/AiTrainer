using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.File.Abstract;
namespace AiTrainer.Web.Domain.Services.File.Concrete
{
    internal class FileCollectionProcessingManager
        : BaseDomainService,
            IFileCollectionProcessingManager
    {
        public FileCollectionProcessingManager(
            IDomainServiceActionExecutor domainServiceActionExecutor
        )
            : base(domainServiceActionExecutor) { }
    }
}
