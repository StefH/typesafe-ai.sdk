using System.Text.Json.Serialization;

namespace TypeSafeAI.Sdk.Contracts;

public sealed class EvaluateRequest
{
    [JsonPropertyName("state")]
    public required object State { get; init; }

    [JsonPropertyName("model")]
    public string Model { get; init; } = "jev-latest";

    [JsonPropertyName("questions")]
    public required Dictionary<string, Question> Questions { get; init; }
}