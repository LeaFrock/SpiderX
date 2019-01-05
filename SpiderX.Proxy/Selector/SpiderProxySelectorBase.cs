using System.Collections.Generic;

namespace SpiderX.Proxy
{
	public abstract class SpiderProxySelectorBase : IProxySelector<SpiderProxy>
	{
		public virtual bool HasNextProxy { get; }

		public virtual int StatusCode { get; }

		public virtual IProxyValidator Validator { get; set; }

		public abstract void Init(IProxyValidator Validator, IEnumerable<SpiderProxy> proxies);

		public abstract void InsertFreshProxies(IEnumerable<SpiderProxy> proxies);

		public abstract SpiderProxy SingleProxy();
	}
}