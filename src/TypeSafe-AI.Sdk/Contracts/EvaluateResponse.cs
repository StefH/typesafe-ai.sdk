using System.Text.Json.Serialization;

namespace TypeSafeAI.Sdk.Contracts;

/// <summary>
/// One answer per question, returned under the same ids you provided.
/// </summary>
public sealed class EvaluateResponse
{
    /// <summary>
    /// The model that performed the evaluation.
    /// </summary>
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    /// <summary>
    /// One Answer per question, keyed by the same ids you used in questions.
    /// </summary>
    [JsonPropertyName("answers")]
    public required Dictionary<string, Answer> Answers { get; init; }

    /// <summary>
    /// Token usage for the request.
    /// </summary>
    [JsonPropertyName("usage")]
    public required Usage Usage { get; init; }
}