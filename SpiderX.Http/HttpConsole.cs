using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using SpiderX.Http.Util;
using SpiderX.Proxy;

namespace SpiderX.Http
{
	public static class HttpConsole
	{
		public const string DefaultPcUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.75 Safari/537.36";
		public const string DefaultMobileUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.75 Safari/537.36";

		public readonly static MediaTypeHeaderValue FormContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded") { CharSet = "UTF-8" };

		public static SpiderWebProxy CreateDefaultSpiderWebProxy(IEnumerable<Uri> proxies)
		{
			DefaultSpiderProxyUriSelector selector = new DefaultSpiderProxyUriSelector()
			{
				ProxyUriInterval = TimeSpan.FromMilliseconds(100)
			};
			selector.Init(proxies);
			return new SpiderWebProxy(selector);
		}

		public static SpiderWebProxy CreateSpiderWebProxy<T>(Func<T> uriSelectorFactory) where T : IProxyUriSelector
		{
			return new SpiderWebProxy(uriSelectorFactory());
		}
	}
}