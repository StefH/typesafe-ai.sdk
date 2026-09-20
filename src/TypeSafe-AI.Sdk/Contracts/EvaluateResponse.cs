using System.Text.Json.Serialization;

namespace TypeSafeAI.Sdk.Contracts;

public sealed class EvaluateResponse
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("answers")]
    public required Dictionary<string, Answer> Answers { get; init; }

    [JsonPropertyName("usage")]
    public required Usage Usage { get; init; }
}