namespace TypeSafeAI.Sdk.Api;

/// <summary>
/// Describes one image file part in a multipart/form-data request.
/// </summary>
public sealed class MultipartImagePart
{
    /// <summary>
    /// The multipart file part name. It must match the asset <c>part</c> value in the request body.
    /// </summary>
    public required string PartName { get; init; }

    /// <summary>
    /// The image stream to upload.
    /// </summary>
    public required Stream ImageStream { get; init; }

    /// <summary>
    /// The file name sent in multipart content disposition.
    /// </summary>
    public string FileName { get; init; } = "image.jpg";

    /// <summary>
    /// The content type sent for the image part.
    /// </summary>
    public string ContentType { get; init; } = "image/jpeg";
}
