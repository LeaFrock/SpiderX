using System.Collections.Generic;
using System.Net;

namespace SpiderX.Proxy
{
	internal interface IProxySelector<T> where T : IWebProxy
	{
		bool HasNextProxy { get; }

		int StatusCode { get; }

		SpiderProxyValidator Validator { get; set; }

		bool CheckLoad(SpiderProxyEntity entity);

		void Init(SpiderProxyValidator Validator, IEnumerable<SpiderProxy> proxies);

		void InsertProxies(IEnumerable<SpiderProxy> proxies);

		T SingleProxy();
	}
}