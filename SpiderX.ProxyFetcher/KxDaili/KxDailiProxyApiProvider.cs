using System;
using SpiderX.Http;

namespace SpiderX.ProxyFetcher
{
	internal sealed class KxDailiProxyApiProvider : ProxyApiProvider
	{
		public KxDailiProxyApiProvider() : base()
		{
			HomePageHost = "ip.kxdaili.com";
			HomePageUrl = "http://ip.kxdaili.com/";
		}

		public const string IpUrl = "http://ip.kxdaili.com/dailiip.html";

		public override SpiderWebClient CreateWebClient()
		{
			throw new NotImplementedException();
		}
	}
}