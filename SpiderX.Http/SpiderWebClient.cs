using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SpiderX.Extensions.Http;

namespace SpiderX.Http
{
	public sealed class SpiderWebClient : HttpClient
	{
		private readonly SocketsHttpHandler _innerHandler;

		public SpiderWebClient() : this(new SocketsHttpHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate })
		{
		}

		public SpiderWebClient(IWebProxy proxy) : this(new SocketsHttpHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate, UseProxy = true, Proxy = proxy })
		{
		}

		public SpiderWebClient(SocketsHttpHandler handler) : base(handler)
		{
			_innerHandler = handler;
			Timeout = TimeSpan.FromMilliseconds(5000);
		}

		public TimeSpan RequestInterval { get; set; } = TimeSpan.FromSeconds(3);

		public async Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod, string requestUrl)
		{
			HttpRequestMessage request = new HttpRequestMessage(httpMethod, requestUrl);
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

		public async Task<string> SendOrRetryAsync(HttpRequestMessage requestMessage, ResponseValidatorBase validator)
		{
			string result = null;
			if (validator == null)
			{
				validator = ResponseValidatorBase.Base;
			}
			for (int i = 0; i < validator.RetryTimes + 1; i++)
			{
				var rMsg = await SendAsync(requestMessage);
				if (rMsg == null || !rMsg.IsSuccessStatusCode)
				{
					continue;
				}
				string tempText = await rMsg.ToTextAsync();
				if (!validator.CheckPass(tempText))
				{
					continue;
				}
				result = tempText;
				break;
			}
			return result;
		}
	}
}