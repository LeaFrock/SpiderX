using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.Models;

namespace SpiderX.Proxy
{
	public sealed class SpiderProxyServer : ProxyServer
	{
		private SpiderProxyServer()
		{
		}

		private int _closeTimes;

		public bool IsClosed => _closeTimes != 0;

		private readonly ConcurrentBag<SpiderProxyServerEventOption> _eventOptions = new ConcurrentBag<SpiderProxyServerEventOption>();

		public IReadOnlyCollection<SpiderProxyServerEventOption> EventOptions => _eventOptions;

		public bool TryAddEventOption(SpiderProxyServerEventOption eventOption)
		{
			if (IsClosed)
			{
				return false;
			}
			if (eventOption is null)
			{
				return false;
			}
			_eventOptions.Add(eventOption);
			eventOption.Bind(this);
			return true;
		}

		public void Close()
		{
			if (IsClosed)
			{
				return;
			}
			if (Interlocked.CompareExchange(ref _closeTimes, 1, 0) != 0)//Ensure that only one thread can move next.
			{
				return;
			}
			if (!_eventOptions.IsEmpty)
			{
				foreach (var eventOpt in _eventOptions)
				{
					eventOpt.UnBind(this);
				}
				_eventOptions.Clear();
			}
			Dispose();
		}

		public static SpiderProxyServer StartNew(ExplicitProxyEndPoint endPoint, SpiderProxyServerEventOption eventOption)
		{
			var server = new SpiderProxyServer();
			server.TryAddEventOption(eventOption);
			server.CertificateManager.TrustRootCertificate(true);//locally trust root certificate used by this proxy.
			server.AddEndPoint(endPoint);
			server.SetAsSystemHttpProxy(endPoint);
			server.SetAsSystemHttpsProxy(endPoint);
			server.Start();
			return server;
		}
	}
}