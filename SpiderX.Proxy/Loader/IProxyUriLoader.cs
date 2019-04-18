using System;

namespace SpiderX.Proxy
{
    public interface IProxyUriLoader
    {
        Predicate<SpiderProxyUriEntity> Condition { get; set; }

        Uri[] Load(int maxCount = 0);
    }
}