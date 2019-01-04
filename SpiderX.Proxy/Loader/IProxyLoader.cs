using System.Collections.Generic;
using System.Net;

namespace SpiderX.Proxy
{
	public interface IProxyLoader
	{
		bool CheckLoad(SpiderProxyEntity entity);

		IList<T> Load<T>() where T : IWebProxy;
	}
}