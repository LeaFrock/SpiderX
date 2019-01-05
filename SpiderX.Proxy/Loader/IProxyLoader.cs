using System;
using System.Collections.Generic;
using System.Net;

namespace SpiderX.Proxy
{
	public interface IProxyLoader
	{
		Predicate<SpiderProxyEntity> Condition { get; set; }

		IList<IWebProxy> Load();
	}
}