using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Extensions;
using BT.Common.FastArray.Proto;
using BT.Common.Helpers;
using BT.Common.Helpers.Extensions;
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
        public virtual DbSet<GlobalFileCollectionConfigEntity> GlobalFileCollectionConfigs { get; set; }
        public virtual DbSet<SharedFileCollectionMemberEntity> SharedFileCollectionMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SharedFileCollectionMemberEntity>(ent =>
            {
                ent
                    .HasOne<FileCollectionEntity>()
                    .WithMany(x => x.SharedFileCollectionMembers)
                    .HasForeignKey(x => x.CollectionId);
            });
            modelBuilder.Entity<FileDocumentMetaDataEntity>(ent =>
            {
                ent.ToTable("file_document_metadata");

                ent.Property(e => e.ExtraData)
                    .HasColumnName("extra_data")
                    .HasColumnType("jsonb")
                    .HasConversion(
                        x => x.SerialiseToJson(null),
                        x => DictionaryHelpers.DeserializeFromJsonString<string, string?>(x)
                    );

                ent.HasOne<FileDocumentEntity>()
                    .WithOne(x => x.MetaData)
                    .HasForeignKey<FileDocumentMetaDataEntity>(e => e.DocumentId);
            });
            modelBuilder.Entity<GlobalFileCollectionConfigEntity>(ent =>
            {
                ent.HasOne<UserEntity>()
                    .WithOne(x => x.GlobalFileCollectionConfig)
                    .HasForeignKey<GlobalFileCollectionConfigEntity>(x => x.UserId);
            });
            modelBuilder.Entity<FileCollectionFaissEntity>(ent =>
            {
                ent.HasOne<FileCollectionEntity>()
                    .WithOne(x => x.FaissStore)
                    .HasForeignKey<FileCollectionFaissEntity>(x => x.CollectionId);

                ent.Property(x => x.DateCreated)
                    .HasConversion(x => x.ToUniversalTime(), x => x.ToUniversalTime());
                ent.Property(x => x.DateModified)
                    .HasConversion(x => x.ToUniversalTime(), x => x.ToUniversalTime());
            });
            modelBuilder.Entity<FileDocumentEntity>(entity =>
            {
                entity.ToTable("file_document", DbConstants.PublicSchema);

                entity
                    .HasOne<FileCollectionEntity>()
                    .WithMany(c => c.Documents)
                    .HasForeignKey(e => e.CollectionId)
                    .HasConstraintName("fk_file_document_collection_id");
                
                entity.Property(e => e.DateCreated)
                    .HasConversion(x => x.ToUniversalTime(), x => x.ToUniversalTime());
            });

            modelBuilder.Entity<UserEntity>(ent =>
            {
                ent.Property(e => e.DateCreated)
                    .HasConversion(x => x.ToUniversalTime(), x => x.ToUniversalTime());
                ent.Property(e => e.DateModified)
                    .HasConversion(x => x.ToUniversalTime(), x => x.ToUniversalTime());
            });

            modelBuilder.Entity<FileCollectionFaissEntity>(ent =>
            {
                ent.Property(e => e.DateCreated)
                    .HasConversion(x => x.ToUniversalTime(), x => x.ToUniversalTime());
                ent.Property(e => e.DateModified)
                    .HasConversion(x => x.ToUniversalTime(), x => x.ToUniversalTime());
            });
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default
        )
        {
            await UpdateDatesOnNewlyAddedOrModifiedAsync();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default
        )
        {
            await UpdateDatesOnNewlyAddedOrModifiedAsync();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateDatesOnNewlyAddedOrModified();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override int SaveChanges()
        {
            UpdateDatesOnNewlyAddedOrModified();
            return base.SaveChanges();
        }

        private async Task UpdateDatesOnNewlyAddedOrModifiedAsync()
        {
            var currentTime = DateTime.UtcNow;
            var updatingEntries = ChangeTracker
                .Entries()
                .FastArrayWhere(e =>
                    e.State == EntityState.Added || e.State == EntityState.Modified
                )
                .ToArray();

            foreach (var entry in updatingEntries)
            {
                if (entry.Entity is FileCollectionFaissEntity foundFaissCollection)
                {
                    if (entry.State == EntityState.Added)
                    {
                        UpdateEntityDatesToToday<
                            FileCollectionFaissEntity,
                            long,
                            FileCollectionFaiss
                        >(
                            foundFaissCollection,
                            [
                                nameof(FileCollectionEntity.DateCreated),
                                nameof(FileCollectionEntity.DateModified),
                            ],
                            currentTime
                        );
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        UpdateEntityDatesToToday<
                            FileCollectionFaissEntity,
                            long,
                            FileCollectionFaiss
                        >(
                            foundFaissCollection,
                            [nameof(FileCollectionEntity.DateModified)],
                            currentTime
                        );
                    }
                }
                else if (entry.Entity is UserEntity foundUser)
                {
                    if (entry.State == EntityState.Added)
                    {
                        var globalAutoSync = new GlobalFileCollectionConfig
                        {
                            UserId = foundUser.Id,
                        };
                        await GlobalFileCollectionConfigs.AddAsync(globalAutoSync.ToEntity());
                        UpdateEntityDatesToToday<UserEntity, Guid, User>(
                            foundUser,
                            [nameof(UserEntity.DateCreated), nameof(UserEntity.DateModified)],
                            currentTime
                        );
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        UpdateEntityDatesToToday<UserEntity, Guid, User>(
                            foundUser,
                            [nameof(UserEntity.DateModified)],
                            currentTime
                        );
                    }
                }
                else if (entry.Entity is FileCollectionEntity foundCollection)
                {
                    if (entry.State == EntityState.Added)
                    {
                        UpdateEntityDatesToToday<FileCollectionEntity, Guid, FileCollection>(
                            foundCollection,
                            [
                                nameof(FileCollectionEntity.DateCreated),
                                nameof(FileCollectionEntity.DateModified),
                            ],
                            currentTime
                        );
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        UpdateEntityDatesToToday<FileCollectionEntity, Guid, FileCollection>(
                            foundCollection,
                            [nameof(FileCollectionEntity.DateCreated)],
                            currentTime
                        );
                    }
                }
                else if (
                    entry is { Entity: FileDocumentEntity foundDocument, State: EntityState.Added }
                )
                {
                    UpdateEntityDatesToToday<FileDocumentEntity, Guid, FileDocument>(
                        foundDocument,
                        [nameof(FileDocumentEntity.DateCreated)],
                        currentTime
                    );
                }
            }
        }

        private void UpdateDatesOnNewlyAddedOrModified()
        {
            var currentTime = DateTime.UtcNow;
            var updatingEntries = ChangeTracker
                .Entries()
                .FastArrayWhere(e =>
                    e.State == EntityState.Added || e.State == EntityState.Modified
                )
                .ToArray();

            foreach (var entry in updatingEntries)
            {
                if (entry.Entity is FileCollectionFaissEntity foundFaissCollection)
                {
                    if (entry.State == EntityState.Added)
                    {
                        UpdateEntityDatesToToday<
                            FileCollectionFaissEntity,
                            long,
                            FileCollectionFaiss
                        >(
                            foundFaissCollection,
                            [
                                nameof(FileCollectionEntity.DateCreated),
                                nameof(FileCollectionEntity.DateModified),
                            ],
                            currentTime
                        );
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        UpdateEntityDatesToToday<
                            FileCollectionFaissEntity,
                            long,
                            FileCollectionFaiss
                        >(
                            foundFaissCollection,
                            [nameof(FileCollectionEntity.DateModified)],
                            currentTime
                        );
                    }
                }
                else if (entry.Entity is UserEntity foundUser)
                {
                    if (entry.State == EntityState.Added)
                    {
                        var globalAutoSync = new GlobalFileCollectionConfig
                        {
                            UserId = foundUser.Id,
                        };
                        GlobalFileCollectionConfigs.Add(globalAutoSync.ToEntity());
                        UpdateEntityDatesToToday<UserEntity, Guid, User>(
                            foundUser,
                            [nameof(UserEntity.DateCreated), nameof(UserEntity.DateModified)],
                            currentTime
                        );
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        UpdateEntityDatesToToday<UserEntity, Guid, User>(
                            foundUser,
                            [nameof(UserEntity.DateModified)],
                            currentTime
                        );
                    }
                }
                else if (entry.Entity is FileCollectionEntity foundCollection)
                {
                    if (entry.State == EntityState.Added)
                    {
                        UpdateEntityDatesToToday<FileCollectionEntity, Guid, FileCollection>(
                            foundCollection,
                            [
                                nameof(FileCollectionEntity.DateCreated),
                                nameof(FileCollectionEntity.DateModified),
                            ],
                            currentTime
                        );
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        UpdateEntityDatesToToday<FileCollectionEntity, Guid, FileCollection>(
                            foundCollection,
                            [nameof(FileCollectionEntity.DateCreated)],
                            currentTime
                        );
                    }
                }
                else if (
                    entry is { Entity: FileDocumentEntity foundDocument, State: EntityState.Added }
                )
                {
                    UpdateEntityDatesToToday<FileDocumentEntity, Guid, FileDocument>(
                        foundDocument,
                        [nameof(FileDocumentEntity.DateCreated)],
                        currentTime
                    );
                }
            }
        }

        private static void UpdateEntityDatesToToday<TEnt, TId, TRuntime>(
            TEnt ent,
            IReadOnlyCollection<string> propertyNames,
            DateTime dateTime
        )
            where TEnt : BaseEntity<TId, TRuntime>
            where TRuntime : class
        {
            var entType = typeof(TEnt);
            foreach (var propName in propertyNames)
            {
                try
                {
                    var propertyToUpdate = entType.GetProperty(propName);
                    if (
                        propertyToUpdate == null
                        || propertyToUpdate.PropertyType != typeof(DateTime)
                    )
                    {
                        continue;
                    }

                    propertyToUpdate.SetValue(ent, dateTime);
                }
                catch
                {
                    //This is ok because we are just trying to update values
                }
            }
        }
    }
}
