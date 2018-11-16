using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpiderX.Http
{
	public sealed class SpiderWebClient : IDisposable
	{
		private HttpClient _innerClient;

		public SpiderWebClient() : this(new HttpClient())
		{
		}

		public SpiderWebClient(HttpMessageHandler handler) : this(new HttpClient(handler))
		{
		}

		public SpiderWebClient(HttpClient targetClient)
		{
			_innerClient = targetClient;
		}

		public async Task<HttpResponseMessage> GetResponse(HttpRequestMessage httpRequest)
		{
			return await _innerClient.SendAsync(httpRequest);
		}

		public void Dispose()
		{
			_innerClient?.Dispose();
		}
	}
}