using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.Extensions;
using BT.Common.WorkflowActivities.Activities.Abstract;
using BT.Common.WorkflowActivities.Activities.Concrete;
using BT.Common.WorkflowActivities.Contexts;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.Workflow.Activities
{
    internal class ValidateDomainModelAgainstOriginalActivity<TModelToValidate, TModelId>
        : BaseActivity<ValidateDomainModelAgainstOriginalActivityContextItem<TModelToValidate, TModelId>, ValidateDomainModelAgainstOriginalActivityReturnItem>
        where TModelToValidate : DomainModel<TModelId>
    {
        public override string Description =>
            "This activity validates a domain model against the original domain model returns a failed activity result if locked values are changed";
        private readonly ILogger<
            ValidateDomainModelAgainstOriginalActivity<TModelToValidate, TModelId>
        > _logger;

        public ValidateDomainModelAgainstOriginalActivity(
            ILogger<ValidateDomainModelAgainstOriginalActivity<TModelToValidate, TModelId>> logger
        )
        {
            _logger = logger;
        }

        public override Task<(ActivityResultEnum ActivityResult, ValidateDomainModelAgainstOriginalActivityReturnItem ActualResult)> ExecuteAsync(
            ValidateDomainModelAgainstOriginalActivityContextItem<TModelToValidate, TModelId> workflowContextItem
        )
        {
            

            if (workflowContextItem.OriginalModel is null || workflowContextItem.NewModel is null)
            {
                return Task.FromResult((ActivityResultEnum.Skip, new ValidateDomainModelAgainstOriginalActivityReturnItem { IsValid = true }));
            }

            var isNewModelOk = workflowContextItem.NewModel.ValidateAgainstOriginal<TModelToValidate, TModelId>(
                workflowContextItem.OriginalModel
            );

            return Task.FromResult(
                (isNewModelOk ? ActivityResultEnum.Success : ActivityResultEnum.Fail, new ValidateDomainModelAgainstOriginalActivityReturnItem { IsValid = isNewModelOk })
            );
        }
    }
    internal record ValidateDomainModelAgainstOriginalActivityContextItem<TModelToValidate, TModelId> : ActivityContextItem
    where TModelToValidate : DomainModel<TModelId>
    {
        public TModelToValidate? NewModel { get; init; }
        public TModelToValidate? OriginalModel { get; init; }
    }
    internal record ValidateDomainModelAgainstOriginalActivityReturnItem : ActivityReturnItem
    {
        public required bool IsValid { get; init; }
    }
}
