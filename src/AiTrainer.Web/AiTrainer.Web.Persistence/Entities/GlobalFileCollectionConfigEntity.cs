using System.ComponentModel.DataAnnotations.Schema;
using AiTrainer.Web.Domain.Models;

namespace AiTrainer.Web.Persistence.Entities;

[Table("global_file_collection_config", Schema = DbConstants.PublicSchema)]
public record GlobalFileCollectionConfigEntity: BaseEntity<long, GlobalFileCollectionConfig>
{
    public bool AutoFaissSync { get; init; }
    public required Guid UserId { get; init; } 
    public override GlobalFileCollectionConfig ToModel()
    {
        return new GlobalFileCollectionConfig
        {
            AutoFaissSync = AutoFaissSync,
            Id = Id,
            UserId = UserId,
        };
    }
}