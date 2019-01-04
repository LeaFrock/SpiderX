using System.Collections.Generic;
using System.Net;

namespace SpiderX.Proxy
{
	public interface IProxySelector<T> where T : IWebProxy
	{
		bool HasNextProxy { get; }

		int StatusCode { get; }

		IProxyLoader Loader { get; set; }

		IProxyValidator Validator { get; set; }

		void Init(IProxyValidator Validator, IEnumerable<T> proxies);

		void InsertFreshProxies(IEnumerable<T> proxies);

		T SingleProxy();
	}
}