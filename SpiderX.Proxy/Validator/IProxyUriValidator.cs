using System;

namespace SpiderX.Proxy
{
	public interface IProxyUriValidator
	{
		Uri TargetUri { get; }

		byte RetryTimes { get; set; }

		bool CheckPass(Uri proxyUri);
	}
}