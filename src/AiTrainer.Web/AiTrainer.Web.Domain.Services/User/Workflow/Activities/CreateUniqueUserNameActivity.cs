using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Persistence.EntityFramework.Entities;
using AiTrainer.Web.Persistence.EntityFramework.Repositories.Abstract;
using BT.Common.WorkflowActivities.Activities.Abstract;
using BT.Common.WorkflowActivities.Activities.Attributes;
using BT.Common.WorkflowActivities.Activities.Concrete;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.User.Workflow.Activities
{
    [DefaultActivityRetry(2, 1)]
    internal class CreateUniqueUserNameActivity : BaseActivity<Models.User, (Models.User?, string?)>
    {
        public override string Description =>
            "This activity is used to create a unique username for a brand new user";

        private readonly IRepository<UserEntity, Guid, Models.User> _repo;
        private readonly ILogger<CreateUniqueUserNameActivity> _logger;

        public CreateUniqueUserNameActivity(
            IRepository<UserEntity, Guid, Models.User> repo,
            ILogger<CreateUniqueUserNameActivity> logger
        )
        {
            _repo = repo;
            _logger = logger;
        }

        public override async Task<(
            ActivityResultEnum ActivityResult,
            (Models.User?, string?) ActualResult
        )> ExecuteAsync(Models.User? workflowContextItem)
        {
            try
            {
                if (workflowContextItem?.Id is null && workflowContextItem?.Username is null)
                {
                    var usernamePrefix = GenerateActualName();
                    var newUsername = $"{usernamePrefix}{GenerateDigitsForUsername()}";
                    var foundUserOfSameUsername = await _repo.GetOne(
                        newUsername,
                        "Username".ToPascalCase()
                    );

                    if (foundUserOfSameUsername is null)
                    {
                        return (ActivityResultEnum.Success, (workflowContextItem, newUsername));
                    }

                    return (
                        ActivityResultEnum.Success,
                        (
                            workflowContextItem,
                            $"{usernamePrefix}{Guid.NewGuid().ToString().Replace("-", "")}"
                        )
                    );
                }

                return (ActivityResultEnum.Skip, (workflowContextItem, null));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Exception occurred while creating a unique username for a new user with message {Message}",
                    ex.Message
                );

                return (ActivityResultEnum.Fail, (workflowContextItem, null));
            }
        }

        private static string GenerateActualName()
        {
            var prefix = Faker.Lorem.GetFirstWord();
            var suffix = Faker.Internet.UserName();

            if (prefix.Contains(' ') || suffix.Contains(' '))
            {
                return GenerateActualName();
            }

            return $"{prefix}{suffix}";
        }

        private static int GenerateDigitsForUsername()
        {
            var random = new Random();

            return random.Next(1000, 9999);
        }
    }
}
