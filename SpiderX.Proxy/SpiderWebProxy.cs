using System;
using System.Net;

namespace SpiderX.Proxy
{
	public sealed class SpiderWebProxy : IWebProxy
	{
		private readonly IProxyUriSelector _selector;

		public SpiderWebProxy(IProxyUriSelector selector)
		{
			_selector = selector ?? throw new ArgumentNullException(nameof(IProxyUriSelector));
		}

		public ICredentials Credentials { get; set; }

		public Uri GetProxy(Uri destination)
		{
			if (destination == null)
			{
				throw new ArgumentNullException(nameof(destination));
			}
			//if(IsBypassed(destination))
			//{
			//	return destination;
			//}
			return _selector.SelectNextProxyUri();
		}

		public bool IsBypassed(Uri host)
		{
			if (host == null)
			{
				throw new ArgumentNullException(nameof(host));
			}
			return false;
		}
	}
}