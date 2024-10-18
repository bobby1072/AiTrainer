using AiTrainer.Web.Common;
using AiTrainer.Web.Persistence.EntityFramework.Contexts;
using AiTrainer.Web.Persistence.Migrations.Abstract;
using AiTrainer.Web.Persistence.Migrations.Concrete;
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

            return services;
        }
    }
}
