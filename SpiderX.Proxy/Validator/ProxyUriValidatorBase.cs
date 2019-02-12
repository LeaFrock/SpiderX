using System;

namespace SpiderX.Proxy
{
	public abstract class ProxyUriValidatorBase : IProxyUriValidator
	{
		public ProxyUriValidatorBase(string urlString) : this(new Uri(urlString))
		{
		}

		public ProxyUriValidatorBase(Uri uri)
		{
			TargetUri = uri ?? throw new ArgumentNullException();
		}

		public Uri TargetUri { get; }

		public byte RetryTimes { get; set; } = 3;

		public abstract bool CheckPass(Uri proxy);
	}
}