using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wam.Proxy.Configuration;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.LoadBalancing;

namespace Wam.Proxy;

public class CustomProxyConfigProvider : IProxyConfigProvider
{
    //private readonly ServicesConfiguration _configuration;

    public CustomProxyConfigProvider(IOptions<ServicesConfiguration> options,
        ILogger<CustomProxyConfigProvider> logger)
    {
        var configuration = options.Value;
        logger.LogInformation("Creating proxy config provider with configuration: {@configuration}", configuration);

        var routeConfigs = new[]
        {
            new RouteConfig
            {
                RouteId = "usersRoute",
                ClusterId = "usersCluster",
                Match = new RouteMatch
                {
                    Path = "/api/users/{**catch-all}"
                }
            },
            new RouteConfig
            {
                RouteId = "gamesRoute",
                ClusterId = "gamesCluster",
                Match = new RouteMatch
                {
                    Path = "/api/games/{**catch-all}"
                }
            },
            new RouteConfig
            {
                RouteId = "scoresRoute",
                ClusterId = "scoresCluster",
                Match = new RouteMatch
                {
                    Path = "/api/scores/{**catch-all}"
                }
            }
        };

        var clusterConfigs = new[]
        {
            new ClusterConfig
            {
                ClusterId = "usersCluster",
                LoadBalancingPolicy = LoadBalancingPolicies.RoundRobin,
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    {
                        "default", new DestinationConfig
                        {
                            Address = $"http://{configuration.UsersService}",
                            Health = $"http://{configuration.UsersService}/health",
                        }
                    }
                }
            },
            new ClusterConfig
            {
                ClusterId = "gamesCluster",
                LoadBalancingPolicy = LoadBalancingPolicies.RoundRobin,
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    {
                        "default", new DestinationConfig
                        {
                            Address = $"http://{configuration.GamesService}",
                            Health = $"http://{configuration.GamesService}/health",
                        }
                    }
                }
            },
            new ClusterConfig
            {
                ClusterId = "scoresCluster",
                LoadBalancingPolicy = LoadBalancingPolicies.RoundRobin,
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    {
                        "default", new DestinationConfig
                        {
                            Address = $"http://{configuration.ScoresService}",
                            Health = $"http://{configuration.ScoresService}/health",
                        }
                    }
                }
            }
        };

        _config = new CustomMemoryConfig(routeConfigs, clusterConfigs);
    }

    private CustomMemoryConfig _config;

    public IProxyConfig GetConfig() => _config;
}