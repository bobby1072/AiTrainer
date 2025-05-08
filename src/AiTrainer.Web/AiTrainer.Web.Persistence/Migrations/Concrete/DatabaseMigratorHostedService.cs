using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Persistence.Migrations.Abstract;
using BT.Common.Polly.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.Persistence.Migrations.Concrete
{
    internal class DatabaseMigratorHostedService : IHostedService
    {
        private readonly IEnumerable<IMigrator> _databaseMigrators;
        private readonly DatabaseMigratorHealthCheck _databaseMigratorHealthCheck;
        private readonly DbMigrationsConfiguration _dbMigrationsConfiguration;
        public DatabaseMigratorHostedService(
            IEnumerable<IMigrator>? databaseMigrators, 
            DatabaseMigratorHealthCheck databaseMigratorHealthCheck,
            IOptions<DbMigrationsConfiguration> dbMigrationsConfiguration)
        {
            _databaseMigrators = databaseMigrators ?? new List<IMigrator>();
            _databaseMigratorHealthCheck = databaseMigratorHealthCheck;
            _dbMigrationsConfiguration = dbMigrationsConfiguration.Value;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(2000, cancellationToken);
            var pipeline = _dbMigrationsConfiguration.ToPipeline();
            
            await pipeline.ExecuteAsync(async _ => await Migrate(), cancellationToken);
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task Migrate()
        {
            foreach (var migrator in _databaseMigrators)
            {
                await migrator.Migrate();
            }
            _databaseMigratorHealthCheck.MigrationCompleted = true;
        }

    }
}
