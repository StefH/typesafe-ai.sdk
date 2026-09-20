using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TypeSafeAI.Sdk.Api;
using TypeSafeAI.Sdk.Services;
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

        services
            .AddITypeSafeApiClient(options =>
            {
                options.BaseAddress = new Uri("https://api.typesafe.ai");
                options.UseSerializer<SystemTextJsonSerializer>();
            })
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var sdkOptions = serviceProvider.GetRequiredService<IOptions<TypeSafeOptions>>().Value;
                client.BaseAddress = sdkOptions.BaseAddress;
            });

        services.AddTransient<ITypeSafeClient, TypeSafeClient>();

        return services;
    }
}