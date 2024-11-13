using AiTrainer.Web.Domain.Services.Abstract;

namespace AiTrainer.Web.Domain.Services.File.Concrete
{
    internal class FileDocumentProcessingManager: BaseDomainService
    {
        public FileDocumentProcessingManager(
    IDomainServiceActionExecutor domainServiceActionExecutor
        )
    : base(domainServiceActionExecutor)
        {
        }
    }
}
