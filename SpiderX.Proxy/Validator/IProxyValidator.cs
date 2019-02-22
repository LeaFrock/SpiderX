using System;
using System.Net.Http;

namespace SpiderX.Proxy
{
	public interface IProxyValidator
	{
		Uri TargetUri { get; }

		byte RetryTimes { get; set; }

		bool CheckPass(HttpResponseMessage response);
	}
}