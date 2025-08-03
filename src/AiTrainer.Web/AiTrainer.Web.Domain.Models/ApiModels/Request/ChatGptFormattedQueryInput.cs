using System.Text.Json;
using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request;

public abstract record BaseChatGptFormattedQueryInput
{
    [JsonPropertyName("definedQueryFormatsEnum")]
    public required int DefinedQueryFormatsEnum { get; init; }
}

public sealed record ChatGptFormattedQueryInput<TInputType>: BaseChatGptFormattedQueryInput where TInputType : ChatQueryInput
{
    [JsonPropertyName("queryInput")] public TInputType QueryInput { get; init; }
}
