using BT.Common.WorkflowActivities.Activities.Abstract;
using BT.Common.WorkflowActivities.Activities.Concrete;
using BT.Common.WorkflowActivities.Contexts;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.Workflow.Activities
{
    internal class ValidateModelActivity<TModelToValidate>
        : BaseActivity<ValidateModelActivityContextItem<TModelToValidate>, ValidateModelActivityReturnItem>
        where TModelToValidate : class
    {
        public override string Description =>
            "This activity validates a model and returns a failed activity result if the validation fails";
        private readonly IValidator<TModelToValidate> _validator;
        private readonly ILogger<ValidateModelActivity<TModelToValidate>> _logger;

        public ValidateModelActivity(
            IValidator<TModelToValidate> validator,
            ILogger<ValidateModelActivity<TModelToValidate>> logger
        )
        {
            _validator = validator;
            _logger = logger;
        }

        public override async Task<(
            ActivityResultEnum ActivityResult,
            ValidateModelActivityReturnItem ActualResult
        )> ExecuteAsync(ValidateModelActivityContextItem<TModelToValidate> workflowContextItem)
        {
            if (workflowContextItem.Model is null)
            {
                return (ActivityResultEnum.Skip, new ValidateModelActivityReturnItem { ValidationResult = null });
            }

            var validationResults = await _validator.ValidateAsync(workflowContextItem.Model);

            if (!validationResults.IsValid)
            {
                _logger.LogDebug(
                    "Validated object of type {TypeName} was invalid",
                    workflowContextItem.GetType().Name
                );
            }

            return (
                validationResults.IsValid ? ActivityResultEnum.Success : ActivityResultEnum.Fail,
                new ValidateModelActivityReturnItem { ValidationResult = validationResults }
            );
        }
    }
    internal record ValidateModelActivityContextItem<TModelToValidate> : ActivityContextItem
        where TModelToValidate : class

    {
        public TModelToValidate? Model { get; init; }
    }
    internal record ValidateModelActivityReturnItem: ActivityReturnItem
    {
        public ValidationResult? ValidationResult { get; init; }
    }
}
