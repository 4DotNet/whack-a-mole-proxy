using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wam.Core.Configuration;
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
            },
            new RouteConfig
            {
                RouteId = "vouchersRoute",
                ClusterId = "vouchersCluster",
                Match = new RouteMatch
                {
                    Path = "/api/vouchers/{**catch-all}"
                }
            },
            new RouteConfig
            {
                RouteId = "realtimeRoute",
                ClusterId = "realtimeCluster",
                Match = new RouteMatch
                {
                    Path = "/api/realtime/{**catch-all}"
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
            },
            new ClusterConfig
            {
                ClusterId = "vouchersCluster",
                LoadBalancingPolicy = LoadBalancingPolicies.RoundRobin,
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    {
                        "default", new DestinationConfig
                        {
                            Address = $"http://{configuration.VouchersService}",
                            Health = $"http://{configuration.ScoresService}/health",
                        }
                    }
                }
            },
            new ClusterConfig
            {
                ClusterId = "realtimeCluster",
                LoadBalancingPolicy = LoadBalancingPolicies.RoundRobin,
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    {
                        "default", new DestinationConfig
                        {
                            Address = $"http://{configuration.RealtimeService}"
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