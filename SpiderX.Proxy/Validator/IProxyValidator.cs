using System;
using System.Net;

namespace SpiderX.Proxy
{
	public interface IProxyValidator
	{
		Uri TargetUri { get; }

		byte RetryTimes { get; set; }

		bool CheckPass(IWebProxy proxy);
	}
}