using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using BT.Common.WorkflowActivities.Activities.Abstract;
using BT.Common.WorkflowActivities.Activities.Attributes;
using BT.Common.WorkflowActivities.Activities.Concrete;
using BT.Common.WorkflowActivities.Contexts;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.User.Workflow.Activities
{
    [DefaultActivityRetry(2, 1)]
    internal class CreateUniqueUserNameActivity : BaseActivity<CreateUniqueUserNameActivityContextItem, CreateUniqueUserNameActivityReturnItem>
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
            CreateUniqueUserNameActivityReturnItem ActualResult
        )> ExecuteAsync(CreateUniqueUserNameActivityContextItem workflowContextItem)
        {
            try
            {
                if (workflowContextItem.UserToStartWith is not null && (workflowContextItem.UserToStartWith.Id is null && workflowContextItem.UserToStartWith.Username is null))
                {
                    var usernamePrefix = GenerateActualName();
                    var newUsername = $"{usernamePrefix}{GenerateDigitsForUsername()}";
                    var foundUserOfSameUsername = await _repo.GetOne(
                        newUsername,
                        "Username".ToPascalCase()
                    );

                    if (foundUserOfSameUsername is null)
                    {
                        return (ActivityResultEnum.Success, new CreateUniqueUserNameActivityReturnItem { NewUsername = newUsername });
                    }

                    return (
                        ActivityResultEnum.Success,
                        new CreateUniqueUserNameActivityReturnItem { NewUsername = $"{usernamePrefix}{Guid.NewGuid().ToString().Replace("-", "")}" }
                    );
                }

                return (ActivityResultEnum.Skip, new CreateUniqueUserNameActivityReturnItem());
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Exception occurred while creating a unique username for a new user with message {Message}",
                    ex.Message
                );

                return (ActivityResultEnum.Fail, new CreateUniqueUserNameActivityReturnItem());
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
    internal record CreateUniqueUserNameActivityContextItem: ActivityContextItem
    {
        public Models.User? UserToStartWith { get; init; } 
    }
    internal record CreateUniqueUserNameActivityReturnItem: ActivityReturnItem
    {
        public string? NewUsername { get; init; }
    }
}
