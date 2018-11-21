using System;
using System.Collections.Generic;
using System.IO;
using SpiderX.Http;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher
{
	internal abstract class ProxyApiProvider
	{
		public string HomePageHost { get; protected set; }

		public string HomePageUrl { get; protected set; }

		public abstract SpiderWebClient CreateWebClient();

		public virtual List<SpiderProxyEntity> GetProxyEntities(string response)
		{
			throw new NotImplementedException();
		}

		public virtual List<SpiderProxyEntity> GetProxyEntities(Stream stream)
		{
			throw new NotImplementedException();
		}
	}
}