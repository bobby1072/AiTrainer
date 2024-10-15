using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AiTrainer.Web.Persistence.Migrations.Concrete;

internal class DatabaseMigratorHealthCheck : IHealthCheck
{
    public const string Name = nameof(DatabaseMigratorHealthCheck);

    public bool MigrationCompleted { get; set; } = false;

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (MigrationCompleted)
        {
            return Task.FromResult(
                HealthCheckResult.Healthy("The database migrator is finished."));
        }

        return Task.FromResult(
            HealthCheckResult.Unhealthy("The database migrator is still running."));
    }
}