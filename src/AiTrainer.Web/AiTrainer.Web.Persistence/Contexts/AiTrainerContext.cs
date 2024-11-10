using AiTrainer.Web.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiTrainer.Web.Persistence.Contexts
{
    internal class AiTrainerContext : DbContext
    {
        public AiTrainerContext(DbContextOptions<AiTrainerContext> options)
            : base(options) { }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<FileDocumentEntity> FileDocuments { get; set; }
    }
}
