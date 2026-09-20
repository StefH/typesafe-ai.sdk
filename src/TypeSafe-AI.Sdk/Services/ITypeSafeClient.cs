using TypeSafeAI.Sdk.Contracts;

namespace TypeSafeAI.Sdk.Services;

public interface ITypeSafeClient
{
    Task<EvaluateResponse> EvaluateAsync(EvaluateRequest request, CancellationToken ct = default);
}
