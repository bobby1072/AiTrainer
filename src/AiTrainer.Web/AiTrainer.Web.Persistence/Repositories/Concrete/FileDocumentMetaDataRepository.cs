using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Extensions;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence.Repositories.Concrete;

internal class FileDocumentMetaDataRepository: BaseRepository<FileDocumentMetaDataEntity, long, FileDocumentMetaData>
{
    public FileDocumentMetaDataRepository(
        IDbContextFactory<AiTrainerContext> dbContextFactory,
        ILogger<FileDocumentMetaDataRepository> logger
    )
        : base(dbContextFactory, logger) { }

    protected override FileDocumentMetaDataEntity RuntimeToEntity(FileDocumentMetaData formInput)
    {
        return formInput.ToEntity();
    }
}