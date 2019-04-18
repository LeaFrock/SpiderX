﻿using System;
using System.Linq;
using SpiderX.Proxy;

namespace SpiderX.Http.Util
{
    public sealed class DefaultProxyUriLoader : IProxyUriLoader
    {
        public int Days { get; set; }

        public IProxyAgent ProxyAgent { get; set; }

        public Predicate<SpiderProxyUriEntity> Condition { get; set; }

        public Uri[] Load(int maxCount)
        {
            var proxyEntities = ProxyAgent.SelectProxyEntities(Condition, Days, maxCount);
            if (proxyEntities.Count < 1)
            {
                return Array.Empty<Uri>();
            }
            return proxyEntities.Select(e => e.Value).ToArray();
        }
    }
}