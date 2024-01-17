using Microsoft.Extensions.Options;
using Wam.Proxy.Configuration;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.LoadBalancing;

namespace Wam.Proxy
{
    public class CustomProxyConfigProvider : IProxyConfigProvider
    {
        //private readonly ServicesConfiguration _configuration;

        public CustomProxyConfigProvider(IOptions<ServicesConfiguration> options)
        {
          var   configuration = options.Value;

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
                        { "default", new DestinationConfig { Address = configuration.UsersService } }
                    }
                },
                new ClusterConfig
                {
                    ClusterId = "gamesCluster",
                    LoadBalancingPolicy = LoadBalancingPolicies.RoundRobin,
                    Destinations = new Dictionary<string, DestinationConfig>
                    {
                        { "default", new DestinationConfig { Address = configuration.GamesService  } }
                    }
                }


            };

            _config = new CustomMemoryConfig(routeConfigs, clusterConfigs);
        }
        private CustomMemoryConfig _config;

        public IProxyConfig GetConfig() => _config;


        //public CustomProxyConfigProvider()
        //{
        //    // Load a basic configuration
        //    // Should be based on your application needs.

        //}
    }
}