using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wam.Proxy.Configuration;

namespace Wam.Proxy.ExtensionMethods;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<ServicesConfiguration>().Bind(configuration.GetSection(ServicesConfiguration.SectionName)); //.ValidateOnStart();
        return services;
    }
}