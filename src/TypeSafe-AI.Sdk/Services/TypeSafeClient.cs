using Microsoft.Extensions.Options;
using TypeSafeAI.Sdk;
using TypeSafeAI.Sdk.Api;
using TypeSafeAI.Sdk.Contracts;

namespace TypeSafeAI.Sdk.Services;

internal sealed class TypeSafeClient(ITypeSafeApiClient apiClient, IOptions<TypeSafeOptions> options) : ITypeSafeClient
{
    private readonly ITypeSafeApiClient _apiClient = apiClient;
    private readonly TypeSafeOptions _options = options.Value;

    public Task<EvaluateResponse> EvaluateAsync(EvaluateRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException("TypeSafe API key is not configured.");
        }

        return _apiClient.EvaluateAsync(request, $"Bearer {_options.ApiKey}", ct);
    }
}
