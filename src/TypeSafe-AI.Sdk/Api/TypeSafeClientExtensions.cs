using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TypeSafeAI.Sdk.Contracts;

namespace TypeSafeAI.Sdk.Api;

/// <summary>
/// Extension methods for <c>ITypeSafeClient</c>.
/// </summary>
public static class TypeSafeClientExtensions
{
    private const string MultipartBoundary = "----TypeSafeAIBoundary";

    /// <summary>
    /// Evaluate a <c>state</c> against a typed <c>Question</c> and get back a structured <c>Answer</c>.
    /// </summary>
    /// <param name="state">The top-level shape of the request.</param>
    /// <param name="key">The key for the question.</param>
    /// <param name="question">The question to evaluate.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The answer for the question.</returns>
    public static async Task<EvaluateResponse> EvaluateQuestionAsync(this ITypeSafeClient client, object state, string key, Question question, CancellationToken cancellationToken = default)
    {
        var request = new EvaluateRequest
        {
            State = state,
            Questions = new Dictionary<string, Question> { { key, question } }
        };
        return await client.EvaluateAsync(request, cancellationToken);
    }

    /// <summary>
    /// Sends an image-enabled request using multipart/form-data with a JSON <c>request</c> part and one or more image file parts.
    /// </summary>
    /// <param name="client">The API client.</param>
    /// <param name="request">The request body that references image assets via <c>assets.*.part</c>.</param>
    /// <param name="imageParts">The multipart image file parts to upload.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>One answer per question.</returns>
    public static async Task<EvaluateResponse> EvaluateImagesAsync(
        this ITypeSafeClient client,
        EvaluateRequest request,
        IEnumerable<MultipartImagePart> imageParts,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(imageParts);

        using var multipart = new MultipartFormDataContent(MultipartBoundary);
        var requestJson = JsonSerializer.Serialize(request);
        multipart.Add(new StringContent(requestJson, Encoding.UTF8, "application/json"), "request");

        var hasAnyImagePart = false;
        foreach (var imagePart in imageParts)
        {
            ArgumentNullException.ThrowIfNull(imagePart);
            if (string.IsNullOrWhiteSpace(imagePart.PartName))
            {
                throw new ArgumentException("Image part name cannot be null or whitespace.", nameof(imageParts));
            }

            var stream = imagePart.ImageStream;
            ArgumentNullException.ThrowIfNull(stream);

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            var imageContent = new StreamContent(stream);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue(imagePart.ContentType);
            multipart.Add(imageContent, imagePart.PartName, imagePart.FileName);
            hasAnyImagePart = true;
        }

        if (!hasAnyImagePart)
        {
            throw new ArgumentException("At least one image part is required.", nameof(imageParts));
        }

        await using var payloadStream = new MemoryStream();
        await multipart.CopyToAsync(payloadStream, cancellationToken);
        payloadStream.Position = 0;

        return await client.EvaluateMultipartAsync(payloadStream, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Sends an image Box request using multipart/form-data with a JSON <c>request</c> part and a file part.
    /// </summary>
    /// <param name="client">The API client.</param>
    /// <param name="request">The request body that references the image asset via <c>assets.*.part</c>.</param>
    /// <param name="imagePartName">The multipart file part name that must match the asset <c>part</c> value.</param>
    /// <param name="imageStream">The image stream to upload.</param>
    /// <param name="fileName">The file name sent in multipart content disposition.</param>
    /// <param name="imageContentType">The image content type.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>One answer per question.</returns>
    public static Task<EvaluateResponse> EvaluateBoxAsync(
        this ITypeSafeClient client,
        EvaluateRequest request,
        string imagePartName,
        Stream imageStream,
        string fileName,
        string imageContentType,
        CancellationToken cancellationToken = default)
    {
        return client.EvaluateImagesAsync(
            request,
            [
                new MultipartImagePart
                {
                    PartName = imagePartName,
                    ImageStream = imageStream,
                    FileName = fileName,
                    ContentType = imageContentType
                }
            ],
            cancellationToken);
    }
}