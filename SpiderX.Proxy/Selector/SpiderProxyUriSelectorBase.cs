using System;
using System.Collections.Generic;

namespace SpiderX.Proxy
{
	public abstract class SpiderProxyUriSelectorBase : IProxyUriSelector
	{
		public virtual bool HasNextProxy { get; }

		public virtual int StatusCode { get; }

		public virtual IProxyUriValidator Validator { get; set; }

		public abstract void Init(IProxyUriValidator Validator, IEnumerable<Uri> proxies);

		public abstract void InsertFreshProxies(IEnumerable<Uri> proxies);

		public abstract Uri SingleProxyUri();
	}
}