using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SpiderX.Extensions.Http;
using SpiderX.Proxy;

namespace SpiderX.Http
{
	public static class HttpConsole
	{
		public const string DefaultPcUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.142 Safari/537.36";
		public const string DefaultMobileUserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 12_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.1.1 Mobile/15E148 Safari/604.1";

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
		public static MediaTypeHeaderValue GetOrAddContentType(string key)
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
			var mediaTypeSpan = key.Substring(0, index);
			var charSetSpan = key.Substring(index + 1);
			return new MediaTypeHeaderValue(mediaTypeSpan) { CharSet = charSetSpan };
		}

		public static string GetResponseTextByProxy(HttpRequestFactory requestFactory, IWebProxySelector proxySelector, Predicate<string> rspValidator = null, byte retryTimes = 9)
		{
			string rspText = null;
			if (retryTimes < 4)
			{
				retryTimes = 4;
			}
			bool isSuccessfulByNormalProxies = false;
			for (byte i = 0; i < retryTimes - 1; i++)
			{
				var proxy = proxySelector.SelectNextProxy();
				rspText = GetResponseTextByProxyAsync(requestFactory, proxy, rspValidator).ConfigureAwait(false).GetAwaiter().GetResult();
				if (rspText != null)
				{
					isSuccessfulByNormalProxies = true;
					proxySelector.OnNormalProxySuccess(proxy);
					break;
				}
				proxySelector.OnNormalProxyFail(proxy);
			}
			if (!isSuccessfulByNormalProxies)
			{
				for (byte i = 0; i < 2; i++)
				{
					bool isAdvancedProxy = proxySelector.TryPreferAdvancedProxy(out var proxy);
					rspText = GetResponseTextByProxyAsync(requestFactory, proxy, rspValidator).ConfigureAwait(false).GetAwaiter().GetResult();
					if (rspText != null)
					{
						if (isAdvancedProxy)
						{
							proxySelector.OnAdvancedProxySuccess(proxy);
						}
						else
						{
							proxySelector.OnNormalProxySuccess(proxy);
						}
						break;
					}
					if (isAdvancedProxy)
					{
						proxySelector.OnAdvancedProxyFail(proxy);
					}
					else
					{
						proxySelector.OnNormalProxyFail(proxy);
					}
				}
			}
			return rspText;
		}

		public static async Task<string> GetResponseTextByProxyAsync(HttpRequestFactory requestFactory, IWebProxySelector proxySelector, Predicate<string> rspValidator = null, byte retryTimes = 9)
		{
			string rspText = null;
			if (retryTimes < 4)
			{
				retryTimes = 4;
			}
			bool isSuccessfulByNormalProxies = false;
			for (byte i = 0; i < retryTimes - 1; i++)
			{
				var proxy = proxySelector.SelectNextProxy();
				rspText = await GetResponseTextByProxyAsync(requestFactory, proxy, rspValidator);
				if (rspText != null)
				{
					isSuccessfulByNormalProxies = true;
					proxySelector.OnNormalProxySuccess(proxy);
					break;
				}
				proxySelector.OnNormalProxyFail(proxy);
			}
			if (!isSuccessfulByNormalProxies)
			{
				for (byte i = 0; i < 2; i++)
				{
					bool isAdvancedProxy = proxySelector.TryPreferAdvancedProxy(out var proxy);
					rspText = await GetResponseTextByProxyAsync(requestFactory, proxy, rspValidator);
					if (rspText != null)
					{
						if (isAdvancedProxy)
						{
							proxySelector.OnAdvancedProxySuccess(proxy);
						}
						else
						{
							proxySelector.OnNormalProxySuccess(proxy);
						}
						break;
					}
					if (isAdvancedProxy)
					{
						proxySelector.OnAdvancedProxyFail(proxy);
					}
					else
					{
						proxySelector.OnNormalProxyFail(proxy);
					}
				}
			}
			return rspText;
		}

		private static async Task<string> GetResponseTextByProxyAsync(HttpRequestFactory requestFactory, IWebProxy proxy, Predicate<string> rspValidator)
		{
			string rspText;
			var client = requestFactory.ClientFactory.Invoke(proxy);
			var reqMsg = requestFactory.MessageFactory.Invoke();
			try
			{
				var rspMsg = await client.SendAsync(reqMsg).ConfigureAwait(false);
				if (rspMsg == null || !rspMsg.IsSuccessStatusCode)
				{
					rspText = null;
				}
				rspText = await rspMsg.ToTextAsync();
			}
			catch
			{
				rspText = null;
			}
			finally
			{
				reqMsg.Dispose();
				client.Dispose();
			}
			if (string.IsNullOrEmpty(rspText))
			{
				return null;
			}
			if (rspValidator?.Invoke(rspText) == false)
			{
				return null;
			}
			return rspText;
		}
	}
}