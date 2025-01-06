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
            modelBuilder.Entity<FileCollectionEntity>().HasMany(fc => fc.FaissStore);

            modelBuilder.Entity<FileDocumentEntity>(entity =>
            {
                entity.ToTable("file_document", DbConstants.PublicSchema);

                entity.Property(e => e.CollectionId).HasColumnName("collection_id");

                entity.Property(e => e.DateCreated).HasColumnName("date_created");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.FileType).HasColumnName("file_type");

                entity.Property(e => e.FileName).HasColumnName("file_name");

                entity.Property(e => e.FileData).HasColumnName("file_data");

                entity.Property(e => e.FileDescription).HasColumnName("file_description");

                entity.HasKey(e => e.Id);

                entity
                    .HasOne<FileCollectionEntity>()
                    .WithMany(c => c.Documents)
                    .HasForeignKey(e => e.CollectionId)
                    .HasConstraintName("fk_file_document_collection_id");
            });
        }
    }
}
