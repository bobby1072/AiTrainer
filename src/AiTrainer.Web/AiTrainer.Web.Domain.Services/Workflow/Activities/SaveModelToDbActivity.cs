using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Models.Extensions;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using BT.Common.WorkflowActivities.Activities.Abstract;
using BT.Common.WorkflowActivities.Activities.Attributes;
using BT.Common.WorkflowActivities.Activities.Concrete;
using BT.Common.WorkflowActivities.Contexts;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.Workflow.Activities
{
    [DefaultActivityRetry(2, 1)]
    internal class SaveModelToDbActivity<TModel, TEnt, TEntId>
        : BaseActivity<SaveModelToDbActivityContextItem<TModel>, SaveModelToDbActivityReturnItem<TModel>>
        where TEnt : BaseEntity<TEntId, TModel>
        where TModel : class
    {
        public override string Description =>
            "This activity is intended to save a entity to the database";
        private readonly IRepository<TEnt, TEntId, TModel> _repo;
        private readonly ILogger<SaveModelToDbActivity<TModel, TEnt, TEntId>> _logger;

        public SaveModelToDbActivity(
            IRepository<TEnt, TEntId, TModel> repo,
            ILogger<SaveModelToDbActivity<TModel, TEnt, TEntId>> logger
        )
        {
            _repo = repo;
            _logger = logger;
        }

        public override async Task<(
            ActivityResultEnum ActivityResult,
            SaveModelToDbActivityReturnItem<TModel> ActualResult
        )> ExecuteAsync(SaveModelToDbActivityContextItem<TModel> workflowContextItem)
        {
            try
            {

                if(workflowContextItem.ModelsToSave.Count < 1)
                {
                    return (ActivityResultEnum.Skip, new SaveModelToDbActivityReturnItem<TModel>());
                }
                var dtosToCreate = new List<TModel>();
                var dtosToUpdate = new List<TModel>();

                foreach (var allEnts in workflowContextItem.ModelsToSave ?? [])
                {
                    var foundId = DomainModelExtensions.GetPropertyValue<object>(
                        allEnts,
                        "Id".ToPascalCase()
                    );

                    if (foundId is null)
                    {
                        dtosToCreate.Add(allEnts);
                    }
                    else
                    {
                        dtosToUpdate.Add(allEnts);
                    }
                }

                if (dtosToCreate.Count > 0 || dtosToUpdate.Count > 0)
                {
                    if (dtosToCreate.Count == 0)
                    {
                        var newUpdatedList = await _repo.Update(dtosToUpdate);
                        return (ActivityResultEnum.Success, new SaveModelToDbActivityReturnItem<TModel> { SavedModels = newUpdatedList });
                    }
                    else if (dtosToUpdate.Count == 0)
                    {
                        var newCreatedList = await _repo.Create(dtosToCreate);
                        return (ActivityResultEnum.Success, new SaveModelToDbActivityReturnItem<TModel> { SavedModels = newCreatedList });
                    }
                    else
                    {
                        var newSavedList = await Task.WhenAll(
                            _repo.Update(dtosToUpdate),
                            _repo.Create(dtosToCreate)
                        );

                        return (
                            ActivityResultEnum.Success,
                            new SaveModelToDbActivityReturnItem<TModel>
                            {
                                SavedModels = new DbSaveResult<TModel>(
                                    newSavedList.SelectMany(x => x.Data!).ToArray()
                                )
                            }
                        );
                    }
                }

                return (ActivityResultEnum.Skip, new SaveModelToDbActivityReturnItem<TModel>());
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "Saving to db finished with exception message {Message}",
                    e.Message
                );

                return (ActivityResultEnum.Fail, new SaveModelToDbActivityReturnItem<TModel>());
            }
        }
    }
    internal record SaveModelToDbActivityContextItem<TModel> : ActivityContextItem where TModel : class
    {
        public required IReadOnlyCollection<TModel> ModelsToSave { get; set; }
    }
    internal record SaveModelToDbActivityReturnItem<TModel> : ActivityReturnItem where TModel : class
    {
        public DbSaveResult<TModel>? SavedModels { get; init; }
    }
}
