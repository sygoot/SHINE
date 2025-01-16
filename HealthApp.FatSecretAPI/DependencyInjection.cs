using FatSecretDotNet;
using FatSecretDotNet.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace HealthApp.FatSecretAPI;
public static class DependencyInjection
{
    public static IServiceCollection AddFatSecretAPI(this IServiceCollection services)
    {
        services.AddSingleton<FatSecretAPI>();
        services.AddSingleton(new FatSecretCredentials()
        {
            ClientId = "2d200df90a344694980d996e3c008c84",
            ClientSecret = "3c7a0a57f4734b80a941bb8ab7e9b3de",
            Scope = "basic" // or premier
        });
        services.AddSingleton<FatSecretClient>();
        return services;
    }

}