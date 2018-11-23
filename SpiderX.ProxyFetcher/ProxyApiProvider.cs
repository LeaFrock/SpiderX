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

		public virtual List<SpiderProxyEntity> GetProxyEntities(string responseText)
		{
			throw new NotImplementedException();
		}

		public virtual List<SpiderProxyEntity> GetProxyEntities<T>(T responseReader) where T : TextReader
		{
			throw new NotImplementedException();
		}
	}
}