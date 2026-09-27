using System.Text.Json.Serialization;

namespace TypeSafeAI.Sdk.Contracts;

/// <summary>
/// Token usage for the request.
/// </summary>
public sealed class Usage
{
    /// <summary>
    /// input_tokens
    /// </summary>
    [JsonPropertyName("input_tokens")]
    public int InputTokens { get; init; }

    /// <summary>
    /// output_tokens
    /// </summary>
    [JsonPropertyName("output_tokens")]
    public int OutputTokens { get; init; }
}