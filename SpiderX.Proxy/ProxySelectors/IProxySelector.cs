using System;
using System.Linq.Expressions;
using System.Net;

namespace SpiderX.Proxy
{
	public interface IProxySelector<T> where T : IWebProxy
	{
		bool HasNextProxy { get; }

		Expression<Func<SpiderProxyEntity, bool>> LoadCondition { get; }

		int LoadFrom(ProxyAgent agent);

		T SingleProxy();
	}
}