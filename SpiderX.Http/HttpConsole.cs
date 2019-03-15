using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using SpiderX.Http.Util;
using SpiderX.Proxy;

namespace SpiderX.Http
{
	public static class HttpConsole
	{
		public const string DefaultPcUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.75 Safari/537.36";
		public const string DefaultMobileUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.75 Safari/537.36";

		/// <summary>
		/// Use for local web debugging tools, like Fiddler etc.
		/// </summary>
		public readonly static IWebProxy LocalWebProxy = new WebProxy("localhost", 8888);

		private readonly static ConcurrentDictionary<string, MediaTypeHeaderValue> _contentTypeDict = new ConcurrentDictionary<string, MediaTypeHeaderValue>();

		public static IReadOnlyDictionary<string, MediaTypeHeaderValue> ContentTypeDict => _contentTypeDict;

		/// <summary>
		/// Get the ContentType from caches by "[mediaType];[charSet]"
		/// </summary>
		/// <param name="key">The key must be in the format "[mediaType];[charSet]"</param>
		/// <returns></returns>
		public static MediaTypeHeaderValue GetContentType(string key)
		{
			return _contentTypeDict.GetOrAdd(key, CreateContentTypeByKey);
		}

		private static MediaTypeHeaderValue CreateContentTypeByKey(string key)
		{
			int index = key.IndexOf(';');
			if (index < 1)
			{
				throw new FormatException();
			}
			ReadOnlySpan<char> span = key.ToCharArray();
			var mediaTypeSpan = span.Slice(0, index);
			var charSetSpan = span.Slice(index + 1);
			return new MediaTypeHeaderValue(new string(mediaTypeSpan)) { CharSet = new string(charSetSpan) };
		}

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