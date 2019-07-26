using System.Net;

namespace SpiderX.Http
{
	public sealed class DefaultSpiderHttpClientFactory : ISpiderHttpClientFactory
	{
		public SpiderHttpClient CreateClient()
		{
			throw new System.NotImplementedException();
		}

		public SpiderHttpClient CreateClient(IWebProxy proxy)
		{
			throw new System.NotImplementedException();
		}
	}
}