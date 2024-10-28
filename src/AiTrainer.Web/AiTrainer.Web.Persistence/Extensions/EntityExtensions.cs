using AiTrainer.Web.Common.Models.DomainModels;
using AiTrainer.Web.Persistence.EntityFramework.Entities;

namespace AiTrainer.Web.Persistence.Extensions
{
    internal static class EntityExtensions
    {
        public static UserEntity ToEntity(this User user)
        {
            var entity = new UserEntity
            {
                DateCreated = user.DateCreated,
                DateModified = user.DateModified,
                Email = user.Email,
                Name = user.Name,
                Username = user.Username
            };
            if (user.Id is not null)
            {
                entity.Id = (Guid)user.Id!;
            }

            return entity;
        }
    }
}
