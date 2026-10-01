using System.Text.Json.Serialization;

namespace TypeSafeAI.Sdk.Contracts;

/// <summary>
/// The top-level shape of every request. Each entry in the <c>questions</c> map is a typed question you name.
/// </summary>
public sealed class EvaluateRequest
{
    /// <summary>
    /// The content to evaluate. A plain string for text, or structured data (object/array) for things like chat logs, records, or the current state of your application.
    /// </summary>
    [JsonPropertyName("state")]
    public required object State { get; init; }

    /// <summary>
    /// The model that handles the request. Use <c>"jev-latest"</c>, TypeSafe’s flagship model.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; init; } = "jev-latest";

    /// <summary>
    /// A map of typed Question objects. You choose each key; answers come back under the same keys.
    /// </summary>
    [JsonPropertyName("questions")]
    public required Dictionary<string, Question> Questions { get; init; }
}