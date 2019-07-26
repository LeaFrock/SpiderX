using System.Net;

namespace SpiderX.Http
{
	public interface ISpiderHttpClientFactory
	{
		SpiderHttpClient CreateClient();

		SpiderHttpClient CreateClient(IWebProxy proxy);
	}
}