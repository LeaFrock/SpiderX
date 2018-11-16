using System.Collections.Generic;
using System.IO;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher
{
	internal abstract class ProxyApiProvider
	{
		public string HomePageHost { get; protected set; }

		public string HomePageUrl { get; protected set; }

		public abstract List<SpiderProxyEntity> GetProxyEntities(string response);

		public virtual List<SpiderProxyEntity> GetProxyEntities(Stream stream)
		{
			return null;
		}
	}
}