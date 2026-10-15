using TypeSafeAI.Sdk.Contracts;
using ZeroAlloc.Rest.Attributes;

namespace TypeSafeAI.Sdk.Api;

/// <summary>
/// Full HTTP API reference for the TypeSafe evaluation endpoint.
/// </summary>
[ZeroAllocRestClient]
public interface ITypeSafeClient
{
    /// <summary>
    /// Evaluate a <c>state</c> against a map of typed <c>Questions</c> and get back structured <c>Answers</c>, one per question.
    /// </summary>
    /// <param name="body">The top-level shape of every request.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>One answer per question.</returns>
    [Post("/v1/systemone")]
    Task<EvaluateResponse> EvaluateAsync([Body] EvaluateRequest body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a multipart/form-data request body for image/box evaluations.
    /// </summary>
    /// <param name="multipartBody">Multipart payload stream containing the <c>request</c> part and image file part(s).</param>
    /// <param name="typeSafeDangerZoneImages">Set to <c>true</c> to enable image and box handling.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>One answer per question.</returns>
    [Post("/v1/systemone")]
    Task<EvaluateResponse> EvaluateMultipartAsync(
        [Body(ContentType = "multipart/form-data; boundary=----TypeSafeAIBoundary")] Stream multipartBody,
        [Header("TypeSafe-Danger-Zone-Images")] string typeSafeDangerZoneImages = "true",
        CancellationToken cancellationToken = default);
}