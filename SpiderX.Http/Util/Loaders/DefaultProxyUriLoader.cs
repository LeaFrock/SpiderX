using System;
using System.Linq;
using SpiderX.Proxy;

namespace SpiderX.Http.Util
{
	public sealed class DefaultProxyUriLoader : IProxyUriLoader
	{
		public int Days { get; set; }

		public Func<ProxyDbContext> DbContextFactory { get; set; }

		public ISpiderProxyUriEntityOption EntityOption { get; set; }

		public Uri[] Load(int maxCount)
		{
			using var dbContext = DbContextFactory.Invoke();
			var proxyEntities = dbContext.SelectProxyEntities(EntityOption, Days, maxCount);
			if (proxyEntities.Count < 1)
			{
				return Array.Empty<Uri>();
			}
			return proxyEntities.Select(e => e.Value).ToArray();
		}
	}
}