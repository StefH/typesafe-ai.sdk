using System.Text.Json.Serialization;

namespace TypeSafeAI.Sdk.Contracts;

/// <summary>
/// Every answer carries a <c>type</c> matching its question. Choice and Score answers also carry a <c>confidence</c> between 0 to 1, derived from the answer’s probability distribution.
/// </summary>
public sealed class Answer
{
    /// <summary>
    /// Every answer carries a <c>type</c> matching its question.
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    /// <summary>
    /// The yes/no answer on a scale from 0 (no) to 1 (yes).
    /// </summary>
    [JsonPropertyName("noul")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Noul { get; init; }

    /// <summary>
    /// The highest-probability option.
    /// </summary>
    [JsonPropertyName("choice")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Choice { get; init; }

    /// <summary>
    /// The probability-weighted answer across the levels; can land between levels.
    /// </summary>
    [JsonPropertyName("score")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? Score { get; init; }

    /// <summary>
    /// How certain the model is, derived from probabilities.
    /// </summary>
    [JsonPropertyName("confidence")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? Confidence { get; init; }

    /// <summary>
    /// Each level number mapped back to its description.
    /// </summary>
    [JsonPropertyName("legend")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? Legend { get; init; }

    /// <summary>
    /// Every option mapped to its probability (floats that sum to 1).
    /// </summary>
    [JsonPropertyName("probabilities")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, float>? Probabilities { get; init; }
}