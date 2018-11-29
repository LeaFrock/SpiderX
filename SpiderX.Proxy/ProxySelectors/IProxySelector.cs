using System.Net;

namespace SpiderX.Proxy
{
	public interface IProxySelector<T> where T : IWebProxy
	{
		bool HasNextProxy { get; }

		bool CheckLoad(SpiderProxyEntity entity);

		int LoadFrom(IProxyAgent agent);

		T SingleProxy();
	}
}