using AiTrainer.Web.Persistence.Migrations.Abstract;
using AiTrainer.Web.Persistence.Migrations.Concrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence
{
    public static class PersistenceServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var migrationStartVersion = configuration.GetSection("Migration").GetSection("StartVersion")?.Value;

            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(migrationStartVersion))
            {
                throw new InvalidDataException("No connection string or migration verison");
            }
            services
            .AddSingleton<IMigrator, DatabaseMigrations>(sp => new DatabaseMigrations(sp.GetRequiredService<ILoggerFactory>().CreateLogger<DatabaseMigrations>(), connectionString, migrationStartVersion));


            services
                .AddHostedService<DatabaseMigratorHostedService>()
                .AddSingleton<DatabaseMigratorHealthCheck>()
                .AddHealthChecks()
                .AddCheck<DatabaseMigratorHealthCheck>(DatabaseMigratorHealthCheck.Name, tags: ["Ready"]);

            return services;
        }
    }
}
