using System.Net;

namespace SpiderX.Proxy
{
	internal interface IProxySelector<T> where T : IWebProxy
	{
		bool HasNextProxy { get; }

		bool CheckLoad(SpiderProxyEntity entity);

		int LoadFrom<TContext>(ProxyAgent<TContext> agent) where TContext : ProxyDbContext;

		T SingleProxy();
	}
}