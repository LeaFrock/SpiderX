using System;

namespace SpiderX.Proxy
{
	public interface IProxyUriLoader
	{
		Uri[] Load(int maxCount = 0);
	}
}