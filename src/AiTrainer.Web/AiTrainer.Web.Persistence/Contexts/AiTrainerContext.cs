using AiTrainer.Web.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiTrainer.Web.Persistence.Contexts
{
    internal class AiTrainerContext : DbContext
    {
        public AiTrainerContext(DbContextOptions<AiTrainerContext> options)
            : base(options) { }

        public virtual DbSet<UserEntity> Users { get; set; }
        public virtual DbSet<FileCollectionEntity> FileCollections { get; set; }
        public virtual DbSet<FileDocumentEntity> FileDocuments { get; set; }
    }
}
