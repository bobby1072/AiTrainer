using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Migrations.Abstract;
using AiTrainer.Web.Persistence.Migrations.Concrete;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.Persistence.Repositories.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace AiTrainer.Web.Persistence.Extensions
{
    public static class PersistenceServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlPersistence(
            this IServiceCollection services,
            IConfiguration configuration,
            bool isDevelopment = false
        )
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var migrationConfigSection = configuration.GetSection(DbMigrationsConfiguration.Key);
            if (!migrationConfigSection.Exists())
            {
                throw new InvalidDataException(ExceptionConstants.MissingEnvVars);
            }
            var migrationConfig = migrationConfigSection.Get<DbMigrationsConfiguration>();
            if (migrationConfig is null)
            {
                throw new InvalidDataException(ExceptionConstants.MissingEnvVars);
            }


            var migrationStartVersion = migrationConfig.StartVersion;

            if (
                string.IsNullOrEmpty(connectionString)
                || string.IsNullOrEmpty(migrationStartVersion)
            )
            {
                throw new InvalidDataException(ExceptionConstants.MissingEnvVars);
            }
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

            services.AddSingleton(Options.Create(migrationConfig));
            
            services.AddSingleton<IMigrator, DatabaseMigrations>(sp => new DatabaseMigrations(
                sp.GetRequiredService<ILoggerFactory>().CreateLogger<DatabaseMigrations>(),
                connectionString,
                migrationStartVersion
            ));

            services
                .AddHostedService<DatabaseMigratorHostedService>()
                .AddSingleton<DatabaseMigratorHealthCheck>()
                .AddHealthChecks()
                .AddCheck<DatabaseMigratorHealthCheck>(
                    DatabaseMigratorHealthCheck.Name,
                    tags: ["Ready"]
                );

            services
                .AddPooledDbContextFactory<AiTrainerContext>(options =>
                    {
                        if (isDevelopment)
                        {
                            options
                                .EnableDetailedErrors()
                                .EnableSensitiveDataLogging();
                        }
                        options
                            .UseSnakeCaseNamingConvention()
                            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                            .UseNpgsql(
                                connectionStringBuilder.ConnectionString,
                                npgOpts =>
                                {
                                    npgOpts.UseQuerySplittingBehavior(
                                        QuerySplittingBehavior.SingleQuery
                                    );
                                }
                            );
                    }
                )
                .AddHealthChecks();

            services
                .AddScoped<IRepository<UserEntity, Guid, User>, UserRepository>()
                .AddScoped<
                    IFileDocumentRepository,
                    FileDocumentRepository
                >()
                .AddScoped<
                    IFileCollectionRepository,
                    FileCollectionRepository
                >()
                .AddScoped<IFileCollectionFaissRepository, FileCollectionFaissRepository>()
                .AddScoped<IRepository<FileDocumentMetaDataEntity, long, FileDocumentMetaData>,
                    FileDocumentMetaDataRepository>()
                .AddScoped<IRepository<GlobalFileCollectionConfigEntity, long, GlobalFileCollectionConfig>, GlobalFileCollectionConfigRepository>()
                .AddScoped<IRepository<SharedFileCollectionMemberEntity, Guid, SharedFileCollectionMember>,SharedFileCollectionMemberRepository>();

            return services;
        }
    }
}
