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
        public virtual DbSet<FileDocumentMetaDataEntity> FileDocumentMetaData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<FileDocumentMetaDataEntity>(ent =>
            {
                ent.ToTable("file_document_metadata");

                ent.HasKey(e => e.Id);

                ent.Property(e => e.Id).HasColumnName("id");

                ent.Property(e => e.DocumentId).HasColumnName("document_id");

                ent.Property(e => e.Title).HasColumnName("title");

                ent.Property(e => e.Author).HasColumnName("author");

                ent.Property(e => e.Subject).HasColumnName("subject");

                ent.Property(e => e.Keywords).HasColumnName("keywords");

                ent.Property(e => e.Creator).HasColumnName("creator");

                ent.Property(e => e.Producer).HasColumnName("producer");

                ent.Property(e => e.CreationDate).HasColumnName("creation_date");

                ent.Property(e => e.ModifiedDate).HasColumnName("modified_date");

                ent.Property(e => e.NumberOfPages).HasColumnName("number_of_pages");

                ent.Property(e => e.IsEncrypted).HasColumnName("is_encrypted");

                ent.Property(e => e.ExtraData).HasColumnName("extra_data").HasColumnType("jsonb");

                ent.HasOne<FileDocumentEntity>()
                    .WithOne(x => x.MetaData)
                    .HasForeignKey<FileDocumentMetaDataEntity>(e => e.DocumentId);
            });
            modelBuilder.Entity<FileCollectionFaissEntity>(ent =>
            {
                ent
                    .HasOne<FileCollectionEntity>()
                    .WithOne(x => x.FaissStore)
                    .HasForeignKey<FileCollectionFaissEntity>(x => x.CollectionId);
            });
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
