using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SpiderX.Http
{
	public sealed class SpiderWebClient : HttpClient
	{
		public SocketsHttpHandler InnerClientHandler { get; }

		public SpiderWebClient() : this(new SocketsHttpHandler() { UseProxy = true })
		{
		}

		public SpiderWebClient(SocketsHttpHandler handler) : base(handler)
		{
			InnerClientHandler = handler;
		}

		public static SpiderWebClient CreateDefault()
		{
			SpiderWebClient client = new SpiderWebClient()
			{
				Timeout = TimeSpan.FromMilliseconds(5000)
			};
			client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
			client.DefaultRequestHeaders.Pragma.Add(new NameValueHeaderValue("no-cache"));
			return client;
		}
	}
}