using System;
using SpiderX.Proxy;

namespace SpiderX.ProxyFilter.Events
{
	internal sealed class ProxyReceiveEventArgs : EventArgs
	{
		public static new ProxyReceiveEventArgs Empty { get; } = new ProxyReceiveEventArgs() { Proxies = Array.Empty<SpiderProxyUriEntity>() };

		public SpiderProxyUriEntity[] Proxies { get; set; }
	}
}