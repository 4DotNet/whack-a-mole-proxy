﻿using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace Wam.Proxy;

internal class CustomMemoryConfig : IProxyConfig
{
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    public CustomMemoryConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
    {
        Routes = routes;
        Clusters = clusters;
        ChangeToken = new CancellationChangeToken(_cts.Token);
    }

    public IReadOnlyList<RouteConfig> Routes { get; }

    public IReadOnlyList<ClusterConfig> Clusters { get; }

    public IChangeToken ChangeToken { get; }

    internal void SignalChange()
    {
        _cts.Cancel();
    }
}
