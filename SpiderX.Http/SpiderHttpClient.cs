using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpiderX.Http
{
	public sealed class SpiderHttpClient : HttpClient
	{
		private readonly SocketsHttpHandler _innerHandler;

		public CookieContainer CookieContainer => _innerHandler.CookieContainer;

		public SpiderHttpClient() : this(new SocketsHttpHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate, UseCookies = false })
		{
		}

		public SpiderHttpClient(IWebProxy proxy) : this(new SocketsHttpHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate, UseProxy = true, Proxy = proxy, UseCookies = false })
		{
		}

		public SpiderHttpClient(SocketsHttpHandler handler) : base(handler)
		{
			_innerHandler = handler;
			Timeout = TimeSpan.FromMilliseconds(5000);
		}

		public async Task<string> GetStringOrRetryAsync(Uri uri, Predicate<string> rspValidator = null, byte retryTimes = 2, int millisecondsDelay = 0)
		{
			string rspText = null;
			for (byte i = 0; i < retryTimes + 1; i++)
			{
				try
				{
					rspText = await GetStringAsync(uri);
					rspText = rspText?.Trim();
				}
				catch
				{
					rspText = null;
				}
				if (!string.IsNullOrEmpty(rspText) && rspValidator?.Invoke(rspText) != false)
				{
					break;
				}
				if (millisecondsDelay > 0)
				{
					await Task.Delay(millisecondsDelay);
				}
			}
			return rspText;
		}
	}
}