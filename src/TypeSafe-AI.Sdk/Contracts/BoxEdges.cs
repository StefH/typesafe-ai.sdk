using System.Text.Json.Serialization;

namespace TypeSafeAI.Sdk.Contracts;

/// <summary>
/// The four edges, <c>top</c>, <c>bottom</c>, <c>left</c>, and <c>right</c>, each from 0 to 1.
/// </summary>
public sealed class BoxEdges
{
    /// <summary>
    /// Fraction of the image height from the top-left corner.
    /// </summary>
    [JsonPropertyName("top")]
    public float Top { get; init; }

    /// <summary>
    /// Fraction of the image height from the top-left corner.
    /// </summary>
    [JsonPropertyName("bottom")]
    public float Bottom { get; init; }

    /// <summary>
    /// Fraction of the image width from the top-left corner.
    /// </summary>
    [JsonPropertyName("left")]
    public float Left { get; init; }

    /// <summary>
    /// Fraction of the image width from the top-left corner.
    /// </summary>
    [JsonPropertyName("right")]
    public float Right { get; init; }
}
