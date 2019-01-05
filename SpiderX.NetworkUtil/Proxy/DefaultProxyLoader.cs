using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using SpiderX.Proxy;

namespace SpiderX.NetworkUtil
{
	public sealed class DefaultProxyLoader : IProxyLoader
	{
		public int Days { get; set; }

		public IProxyAgent ProxyAgent { get; set; }

		public Predicate<SpiderProxyEntity> Condition { get; set; }

		public IList<IWebProxy> Load()
		{
			var proxyEntities = ProxyAgent.SelectProxyEntities(Condition, Days, 50000);
			if (proxyEntities.Count < 1)
			{
				return Array.Empty<IWebProxy>();
			}
			return proxyEntities.Select(e => e.ReadOnlyValue).ToList();
		}
	}
}