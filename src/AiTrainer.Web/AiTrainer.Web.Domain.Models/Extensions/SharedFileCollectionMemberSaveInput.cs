using System.Text.Json;
using AiTrainer.Web.Domain.Models.ApiModels.Request;

namespace AiTrainer.Web.Domain.Models.Extensions;

public static class SharedFileCollectionMemberSaveInputExtensions
{
    public static SharedFileCollectionMemberSaveInput ToSanitisedInput(this SharedFileCollectionMemberRawSaveInput genericMemberSaveInput)
    {
        var singleMemberInput = new List<SharedFileCollectionSingleMemberSaveInput>();

        foreach (var member in genericMemberSaveInput.MembersToShareTo)
        {
            if (!string.IsNullOrEmpty(member.RootElement
                    .GetProperty(SharedFileCollectionSingleMemberEmailSaveInput.EmailJsonName)
                    .GetString()))
            {
                singleMemberInput.Add(
                    JsonSerializer
                        .Deserialize<SharedFileCollectionSingleMemberEmailSaveInput>(member.RootElement.GetRawText()) ?? throw new JsonException($"Failed to deserialize json input to {nameof(SharedFileCollectionMemberSaveInput)}")
                );
            }
            else if (!string.IsNullOrEmpty(member.RootElement
                    .GetProperty(SharedFileCollectionSingleMemberUserIdSaveInput.UserIdJsonName)
                    .GetString()))
            {
                singleMemberInput.Add(
                    JsonSerializer
                        .Deserialize<SharedFileCollectionSingleMemberUserIdSaveInput>(member.RootElement.GetRawText()) ?? throw new JsonException($"Failed to deserialize json input to {nameof(SharedFileCollectionMemberSaveInput)}")
                );
            }
        }
        

        return new SharedFileCollectionMemberSaveInput
        {
            CollectionId = genericMemberSaveInput.CollectionId,
            MembersToShareTo = singleMemberInput.ToArray(),
        };
    }
} 