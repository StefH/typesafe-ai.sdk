using System.Text.Json.Serialization;

namespace TypeSafeAI.Sdk.Contracts;

/// <summary>
/// The images in the request, keyed by reference string.
/// </summary>
public sealed class Asset
{
    /// <summary>
    /// The kind of asset. Optional. If you send it, it must be <c>"image"</c>.
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; } = "image";

    /// <summary>
    /// In a multipart request, the name of the file part that holds the image.
    /// </summary>
    [JsonPropertyName("part")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Part { get; init; }

    /// <summary>
    /// The image’s format, in lowercase. Required with <c>data_base64</c>. Optional with <c>part</c>.
    /// </summary>
    [JsonPropertyName("format")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Format { get; init; }

    /// <summary>
    /// The image file, base64-encoded. Required unless <c>part</c> is set.
    /// </summary>
    [JsonPropertyName("data_base64")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Base64 { get; init; }
}
