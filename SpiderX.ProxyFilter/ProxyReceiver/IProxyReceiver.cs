using System;
using System.Threading.Tasks;
using SpiderX.ProxyFilter.Events;

namespace SpiderX.ProxyFilter
{
	internal interface IProxyReceiver
	{
		EventHandler<ProxyReceiveEventArgs> OnProxyReceived { get; set; }

		Task InitializeAsync();

		void Disable();
	}
}