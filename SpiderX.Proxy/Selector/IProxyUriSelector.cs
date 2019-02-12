using System;

namespace SpiderX.Proxy
{
	public interface IProxyUriSelector
	{
		Uri SingleProxyUri();
	}
}