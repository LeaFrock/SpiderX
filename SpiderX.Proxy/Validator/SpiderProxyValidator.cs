using System;
using System.Net;

namespace SpiderX.Proxy
{
	public abstract class SpiderProxyValidator : IProxyValidator
	{
		public SpiderProxyValidator(string urlString) : this(new Uri(urlString))
		{
		}

		public SpiderProxyValidator(Uri uri)
		{
			TargetUri = uri ?? throw new ArgumentNullException();
		}

		public Uri TargetUri { get; }

		public byte RetryTimes { get; set; } = 3;

		public abstract bool CheckPass(IWebProxy proxy);
	}
}