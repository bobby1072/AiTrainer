using AiTrainer.Web.Persistence.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiTrainer.Web.Persistence.EntityFramework.Contexts
{
    internal class AiTrainerContext : DbContext
    {
        public AiTrainerContext(DbContextOptions<AiTrainerContext> options)
            : base(options) { }

        public DbSet<UserEntity> Users { get; set; }
    }
}
