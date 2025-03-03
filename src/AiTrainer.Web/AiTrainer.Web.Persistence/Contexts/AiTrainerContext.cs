using AiTrainer.Web.Domain.Models;
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
                ent.HasOne<FileCollectionEntity>()
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

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default
        )
        {
            var updatingEntries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in updatingEntries)
            {
                if (entry.Entity is UserEntity foundUser)
                {
                    if (entry.State == EntityState.Added)
                    {
                        UpdateEntityDatesToToday<UserEntity, Guid, User>(foundUser, [nameof(UserEntity.DateCreated), nameof(UserEntity.DateModified)]);
                    }
                    else
                    {
                        UpdateEntityDatesToToday<UserEntity, Guid, User>(foundUser, [nameof(UserEntity.DateModified)]);
                    }
                }
                else if (entry.Entity is FileCollectionEntity foundCollection)
                {
                    if (entry.State == EntityState.Added)
                    {
                        UpdateEntityDatesToToday<FileCollectionEntity, Guid, FileCollection>(foundCollection, [nameof(FileCollectionEntity.DateCreated), nameof(FileCollectionEntity.DateModified)]);
                    }
                    else
                    {
                        UpdateEntityDatesToToday<FileCollectionEntity, Guid, FileCollection>(foundCollection, [nameof(FileCollectionEntity.DateCreated)]);
                    }
                }
                else if (entry.Entity is FileDocumentEntity foundDocument)
                {
                    if (entry.State == EntityState.Added)
                    {
                        UpdateEntityDatesToToday<FileDocumentEntity, Guid, FileDocument>(foundDocument, [nameof(FileDocumentEntity.DateCreated)]);
                    }
                }
            }
            
            
            return await base.SaveChangesAsync(cancellationToken);
        }


        private static void UpdateEntityDatesToToday<TEnt, TId, TRuntime>(TEnt ent, IReadOnlyCollection<string> propertyNames) 
            where TEnt : BaseEntity<TId, TRuntime>
            where TRuntime : class
        {
            foreach (var dateType in propertyNames)
            {
                try
                {
                    UpdateObjectDatesToToday(ent, dateType);
                }
                catch
                {
                    //This is ok because we are just trying to update values
                }
            }
        }

        private static void UpdateObjectDatesToToday<TEnt>(TEnt ent, string propertyName)
        {
            var propertyToUpdate = typeof(TEnt).GetProperty(propertyName);
            if (propertyToUpdate == null || propertyToUpdate.PropertyType != typeof(DateTime))
            {
                return;
            }
            
            propertyToUpdate.SetValue(ent, DateTime.UtcNow);
        }
    }
    
}
