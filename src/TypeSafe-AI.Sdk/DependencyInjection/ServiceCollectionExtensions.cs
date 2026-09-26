using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TypeSafeAI.Sdk.Api;
using ZeroAlloc.Rest.SystemTextJson;

namespace TypeSafeAI.Sdk.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTypeSafeSdk(this IServiceCollection services, Action<TypeSafeOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        services
            .AddOptions<TypeSafeOptions>()
            .Configure(configure)
            .ValidateDataAnnotations();

        var sdkOptions = services.BuildServiceProvider().GetRequiredService<IOptions<TypeSafeOptions>>().Value;

        services
            .AddITypeSafeClient(options =>
            {
                options.BaseAddress = sdkOptions.BaseAddress;
                options.UseSerializer<SystemTextJsonSerializer>();
            })
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sdkOptions.ApiKey}");
            });

        return services;
    }
}