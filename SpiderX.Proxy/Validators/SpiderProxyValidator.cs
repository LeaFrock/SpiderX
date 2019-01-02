using System;
using System.Net;

namespace SpiderX.Proxy
{
	public abstract class SpiderProxyValidator
	{
		public SpiderProxyValidator(string urlString) : this(new Uri(urlString))
		{
		}

		public SpiderProxyValidator(Uri uri)
		{
			TargetUri = uri ?? throw new ArgumentNullException();
		}

		public Uri TargetUri { get; }

		/// <summary>
		/// Between 1 and 254
		/// </summary>
		public byte RetryTimes { get; set; } = 3;

		public abstract bool CheckPass(IWebProxy proxy);
	}
}