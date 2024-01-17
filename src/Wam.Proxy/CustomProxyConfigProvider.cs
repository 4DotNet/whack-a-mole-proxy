﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wam.Proxy.Configuration;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.LoadBalancing;

namespace Wam.Proxy
{
    public class CustomProxyConfigProvider : IProxyConfigProvider
    {
        //private readonly ServicesConfiguration _configuration;

        public CustomProxyConfigProvider(IOptions<ServicesConfiguration> options,
            ILogger<CustomProxyConfigProvider> logger)
        {
            var configuration = options.Value;
            logger.LogInformation("Creating proxy config provider with configuration: {@configuration}", configuration);

            var shortLinksRouteConfig = new RouteConfig
            {
                RouteId = "usersRoute",
                ClusterId = "usersCluster",
                Match = new RouteMatch
                {
                    Path = "/api/users/{**catch-all}"
                }
            };

            var hitsRouteConfig = new RouteConfig
            {
                RouteId = "gamesRoute",
                ClusterId = "gamesCluster",
                Match = new RouteMatch
                {
                    Path = "/api/games/{**catch-all}"
                }
            };



            var routeConfigs = new[] { shortLinksRouteConfig, hitsRouteConfig };

            var clusterConfigs = new[]
            {
                new ClusterConfig
                {
                    ClusterId = "usersCluster",
                    LoadBalancingPolicy = LoadBalancingPolicies.RoundRobin,
                    Destinations = new Dictionary<string, DestinationConfig>
                    {
                        { "default", new DestinationConfig { Address = $"http://{configuration.UsersService}:8080" } }
                    }
                },
                new ClusterConfig
                {
                    ClusterId = "gamesCluster",
                    LoadBalancingPolicy = LoadBalancingPolicies.RoundRobin,
                    Destinations = new Dictionary<string, DestinationConfig>
                    {
                        { "default", new DestinationConfig { Address = $"http://{configuration.GamesService}:8080" } }
                    }
                }


            };

            _config = new CustomMemoryConfig(routeConfigs, clusterConfigs);
        }

        private CustomMemoryConfig _config;

        public IProxyConfig GetConfig() => _config;
    }
}