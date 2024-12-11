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
using Npgsql;

namespace AiTrainer.Web.Persistence
{
    public static class PersistenceServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlPersistence(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var migrationStartVersion = configuration
                .GetSection("Migration")
                .GetSection("StartVersion")
                ?.Value;

            if (
                string.IsNullOrEmpty(connectionString)
                || string.IsNullOrEmpty(migrationStartVersion)
            )
            {
                throw new InvalidDataException(ExceptionConstants.MissingEnvVars);
            }
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

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
                    options
                        .UseSnakeCaseNamingConvention()
                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                        .UseNpgsql(
                            connectionStringBuilder.ConnectionString,
                            options =>
                            {
                                options.UseQuerySplittingBehavior(
                                    QuerySplittingBehavior.SingleQuery
                                );
                            }
                        )
                )
                .AddHealthChecks();

            services
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IFileDocumentRepository, FileDocumentRepository>()
                .AddScoped<IFileCollectionRepository, FileCollectionRepository>()
                .AddScoped<
                    IRepository<FileCollectionFaissEntity, long, FileCollectionFaiss>,
                    FileCollectionFaissRepository
                >()
                .AddScoped<ISolicitedDeviceTokenRepository, SolicitedDeviceTokenRepository>();

            return services;
        }
    }
}
