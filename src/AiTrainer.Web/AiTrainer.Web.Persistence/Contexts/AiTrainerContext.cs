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
        public virtual DbSet<FileCollectionFaissEntity> FileCollectionFaiss { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<FileCollectionEntity>()
                .HasMany(fc => fc.Children);

            modelBuilder
                .Entity<FileCollectionEntity>()
                .HasMany(fc => fc.FaissStore);

            modelBuilder
                .Entity<FileCollectionEntity>()
                .HasMany(x => x.Documents);
        }
    }
}
