using System;
using System.Collections.Generic;

namespace SpiderX.Proxy
{
    public abstract class SpiderProxyUriSelectorBase : IProxyUriSelector
    {
        public virtual bool HasNextProxy { get; }

        public virtual int StatusCode { get; }

        public abstract void Init(IEnumerable<Uri> proxies);

        public abstract void InsertFreshProxies(IEnumerable<Uri> proxies);

        public abstract Uri SelectNextProxyUri();

        public abstract Uri SelectGoodProxyUri();
    }
}