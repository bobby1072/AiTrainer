namespace AiTrainer.Web.Persistence.Migrations.Abstract
{
    internal interface IMigrator
    {
        public Task Migrate();
    }
}