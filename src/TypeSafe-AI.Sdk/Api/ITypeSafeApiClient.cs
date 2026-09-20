using TypeSafeAI.Sdk.Contracts;
using ZeroAlloc.Rest.Attributes;

namespace TypeSafeAI.Sdk.Api;

[ZeroAllocRestClient]
public interface ITypeSafeApiClient
{
    [Post("/v1/systemone")]
    Task<EvaluateResponse> EvaluateAsync([Body] EvaluateRequest body, [Header("Authorization")] string authorization, CancellationToken cancellationToken = default);
}