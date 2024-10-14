namespace AiTrainer.Web.Persistence.Migrations.Abstract
{
    public interface IMigrator
    {
        public Task Migrate();
    }
}