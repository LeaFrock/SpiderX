using System;
using System.Net;
using System.Net.Http;

namespace SpiderX.Http
{
	public sealed class HttpRequestFactory
	{
		public HttpRequestFactory(Func<IWebProxy, HttpClient> clientFactory, Func<HttpRequestMessage> messageFactory)
		{
			ClientFactory = clientFactory;
			MessageFactory = messageFactory;
		}

		public Func<IWebProxy, HttpClient> ClientFactory { get; }

		public Func<HttpRequestMessage> MessageFactory { get; set; }

		public static HttpMessageHandler GetDefaultHttpHandler(IWebProxy proxy, double milliseconds = 5000)
		{
			TimeSpan timeout = TimeSpan.FromMilliseconds(milliseconds);
			return GetDefaultHttpHandler(proxy, timeout);
		}

		public static HttpMessageHandler GetDefaultHttpHandler(IWebProxy proxy, TimeSpan timeout)
		{
			var handler = new SocketsHttpHandler()
			{
				UseProxy = true,
				Proxy = proxy,
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
				ConnectTimeout = timeout,
				Expect100ContinueTimeout = timeout,
				ResponseDrainTimeout = timeout
			};
			return handler;
		}
	}
}