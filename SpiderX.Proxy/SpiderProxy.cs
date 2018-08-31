using System;
using System.Net;

namespace SpiderX.Proxy
{
	public sealed class SpiderProxy : WebProxy
	{
		public SpiderProxy(Uri Address) : base(Address)
		{
		}
	}
}