using System;
using System.Collections.Generic;

namespace SpiderX.Proxy
{
	public abstract class SpiderProxyUriSelectorBase : IProxyUriSelector
	{
		public virtual bool HasNextProxy { get; }

		public virtual int StatusCode { get; }

		public virtual IProxyValidator Validator { get; set; }

		public abstract void Init(IProxyValidator Validator, IEnumerable<Uri> proxies);

		public abstract void InsertFreshProxies(IEnumerable<Uri> proxies);

		public abstract Uri SingleProxyUri();
	}
}