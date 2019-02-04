using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SpiderX.Extensions.Http;

namespace SpiderX.Http
{
	public sealed class SpiderWebClient : HttpClient
	{
		public SocketsHttpHandler InnerClientHandler { get; }

		public SpiderWebClient() : this(new SocketsHttpHandler() { UseProxy = true, AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate })
		{
		}

		public SpiderWebClient(SocketsHttpHandler handler) : base(handler)
		{
			InnerClientHandler = handler;
		}

		public TimeSpan RequestInterval { get; set; } = TimeSpan.FromSeconds(3);

		public void SetProxy(IWebProxy proxy)
		{
			InnerClientHandler.UseProxy = proxy != null;
			InnerClientHandler.Proxy = proxy;
		}

		public async Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod, string requestUrl)
		{
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
			try
			{
				return await SendAsync(request);
			}
			catch (Exception)
			{
				request.Dispose();
				return null;
			}
		}

		public async Task<string> GetOrRetryAsync(HttpRequestMessage requestMessage, int retryTimes, Predicate<string> passFunc)
		{
			string result = null;
			if (passFunc == null)
			{
				for (int i = 0; i < retryTimes; i++)
				{
					var rMsg = await SendAsync(requestMessage);
					if (rMsg == null || !rMsg.IsSuccessStatusCode)
					{
						continue;
					}
					string tempText = await rMsg.ToHtmlTextAsync();
					if (string.IsNullOrEmpty(tempText))
					{
						continue;
					}
					result = tempText;
					break;
				}
			}
			else
			{
				for (int i = 0; i < retryTimes; i++)
				{
					var rMsg = await SendAsync(requestMessage);
					if (rMsg == null || !rMsg.IsSuccessStatusCode)
					{
						continue;
					}
					string tempText = await rMsg.ToHtmlTextAsync();
					if (string.IsNullOrEmpty(tempText) || !passFunc(tempText))
					{
						continue;
					}
					result = tempText;
					break;
				}
			}
			return result;
		}

		public static SpiderWebClient CreateDefault()
		{
			SpiderWebClient client = new SpiderWebClient()
			{
				Timeout = TimeSpan.FromMilliseconds(5000)
			};
			return client;
		}
	}
}