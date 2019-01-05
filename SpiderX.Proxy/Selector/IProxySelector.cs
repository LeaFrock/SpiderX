using System.Net;

namespace SpiderX.Proxy
{
	public interface IProxySelector<out T> where T : IWebProxy
	{
		T SingleProxy();
	}
}