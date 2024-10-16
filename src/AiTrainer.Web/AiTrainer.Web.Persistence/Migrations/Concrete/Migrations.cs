using AiTrainer.Web.Persistence.Migrations.Abstract;
using EvolveDb;
using EvolveDb.Migration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace AiTrainer.Web.Persistence.Migrations.Concrete
{
    internal class DatabaseMigrations : IMigrator
    {
        private readonly ILogger<DatabaseMigrations> _logger;
        private readonly string _connectionString;
        private readonly string _startVersion;
        public DatabaseMigrations(ILogger<DatabaseMigrations> logger, string connectionUrl, string startVersion)
        {
            _logger = logger;
            _connectionString = connectionUrl;
            _startVersion = startVersion;
        }
        public Task Migrate()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var evolve = new Evolve(connection, msg => _logger.LogInformation(msg))
            {
                EmbeddedResourceAssemblies = new[] { typeof(DatabaseMigrations).Assembly },
                EnableClusterMode = true,
                StartVersion = new MigrationVersion(_startVersion),
                IsEraseDisabled = true,
                MetadataTableName = "migrations_changelog",
                OutOfOrder = true
            };
            evolve.Migrate();
            return Task.CompletedTask;
        }
    }
}
