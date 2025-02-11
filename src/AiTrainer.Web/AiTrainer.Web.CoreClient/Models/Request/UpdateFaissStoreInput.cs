﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Request;

public record UpdateFaissStoreInput: BaseCoreClientRequestData
{
    [JsonIgnore] 
    public byte[] FileInput { get; init; } = [];
    [JsonPropertyName("docStore")]
    public required JsonDocument DocStore { get; init; }
    [JsonPropertyName("newDocuments")]
    public required CreateFaissStoreInput NewDocuments { get; init; }
}