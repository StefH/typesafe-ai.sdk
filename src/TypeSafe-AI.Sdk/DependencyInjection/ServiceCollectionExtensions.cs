using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TypeSafeAI.Sdk.Api;
using ZeroAlloc.Rest.SystemTextJson;

namespace TypeSafeAI.Sdk.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the TypeSafe SDK to the service collection with the specified configuration.
    /// </summary>
    /// <param name="services">The service collection to add the SDK to.</param>
    /// <param name="configure">An action to configure the TypeSafeOptions.</param>
    /// <returns>The updated service collection.</returns>
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
            .ConfigureHttpClient(client =>
            {
                if (!string.IsNullOrEmpty(sdkOptions.ApiKey))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sdkOptions.ApiKey}");
                }
            });

        return services;
    }
}