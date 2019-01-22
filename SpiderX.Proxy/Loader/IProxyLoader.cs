using System;
using System.Net;

namespace SpiderX.Proxy
{
	public interface IProxyLoader
	{
		Predicate<SpiderProxyEntity> Condition { get; set; }

		IWebProxy[] Load();
	}
}