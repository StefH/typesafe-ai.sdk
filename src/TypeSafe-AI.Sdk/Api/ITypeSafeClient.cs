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
    /// Evaluate a <c>state</c> against a map of typed <c>questions</c> and get back structured <c>answers</c>, one per question.
    /// </summary>
    /// <param name="body">The top-level shape of every request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>One answer per question, returned under the same ids you provided.</returns>
    [Post("/v1/systemone")]
    Task<EvaluateResponse> EvaluateAsync([Body] EvaluateRequest body, CancellationToken cancellationToken = default);
}