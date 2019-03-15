using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SpiderX.Extensions.Http;

namespace SpiderX.Http
{
	public sealed class SpiderWebClient : HttpClient
	{
		private readonly SocketsHttpHandler _innerHandler;

		public SpiderWebClient() : this(new SocketsHttpHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate, UseCookies = false })
		{
		}

		public SpiderWebClient(IWebProxy proxy) : this(new SocketsHttpHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate, UseProxy = true, Proxy = proxy, UseCookies = false })
		{
		}

		public SpiderWebClient(SocketsHttpHandler handler) : base(handler)
		{
			_innerHandler = handler;
			Timeout = TimeSpan.FromMilliseconds(5000);
		}

		public TimeSpan RequestInterval { get; set; } = TimeSpan.FromSeconds(5);

		public async Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod, string requestUrl)
		{
			HttpRequestMessage request = new HttpRequestMessage(httpMethod, requestUrl);
			return await SendAsync(request, true);
		}

		public async Task<string> GetOrRetryAsync(Uri uri, ResponseValidatorBase validator)
		{
			if (validator == null)
			{
				validator = ResponseValidatorBase.Base;
			}
			string result = null;
			if (RequestInterval > TimeSpan.Zero)
			{
				for (int i = 0; i < validator.RetryTimes + 1; i++)
				{
					try
					{
						var text = await GetStringAsync(uri);
						result = text?.Trim();
					}
					catch (Exception)
					{
						result = null;
						Thread.Sleep(RequestInterval);
						continue;
					}
					if (validator.CheckPass(result))
					{
						break;
					}
					Thread.Sleep(RequestInterval);
				}
			}
			else
			{
				for (int i = 0; i < validator.RetryTimes + 1; i++)
				{
					try
					{
						var text = await GetStringAsync(uri);
						result = text?.Trim();
					}
					catch (Exception)
					{
						result = null;
						continue;
					}
					if (validator.CheckPass(result))
					{
						break;
					}
				}
			}
			return result;
		}

		public async Task<string> SendOrRetryAsync(HttpRequestMessage requestMessage, ResponseValidatorBase validator, bool disposeRequestIfFail = true)
		{
			string result = null;
			if (validator == null)
			{
				validator = ResponseValidatorBase.Base;
			}
			HttpResponseMessage responseMessage = null;
			if (RequestInterval > TimeSpan.Zero)
			{
				for (int i = 0; i < validator.RetryTimes + 1; i++)
				{
					responseMessage = await SendAsync(requestMessage, false);
					if (responseMessage == null || !responseMessage.IsSuccessStatusCode)
					{
						Thread.Sleep(RequestInterval);
						continue;
					}
					string tempText = (await responseMessage.ToTextAsync())?.Trim();
					if (validator.CheckPass(tempText))
					{
						result = tempText;
						break;
					}
					Thread.Sleep(RequestInterval);
				}
			}
			else
			{
				for (int i = 0; i < validator.RetryTimes + 1; i++)
				{
					responseMessage = await SendAsync(requestMessage, false);
					if (responseMessage == null || !responseMessage.IsSuccessStatusCode)
					{
						continue;
					}
					string tempText = (await responseMessage.ToTextAsync())?.Trim();
					if (validator.CheckPass(tempText))
					{
						result = tempText;
						break;
					}
				}
			}
			if (result == null)
			{
				if (disposeRequestIfFail)
				{
					requestMessage.Dispose();
				}
				responseMessage?.Dispose();
			}
			return result;
		}

		private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, bool disposeRequestIfFail)
		{
			try
			{
				return await SendAsync(request);
			}
			catch (Exception)
			{
				if (disposeRequestIfFail)
				{
					request.Dispose();
				}
				return null;
			}
		}
	}
}