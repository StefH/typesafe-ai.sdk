using TypeSafeAI.Sdk.Contracts;
using ZeroAlloc.Rest.Attributes;

namespace TypeSafeAI.Sdk.Api;

[ZeroAllocRestClient]
public interface ITypeSafeClient
{
    [Post("/v1/systemone")]
    Task<EvaluateResponse> EvaluateAsync([Body] EvaluateRequest body, CancellationToken cancellationToken = default);
}