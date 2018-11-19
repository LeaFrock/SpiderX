using System.Net;

namespace SpiderX.Proxy
{
	public sealed class SpiderProxy : WebProxy
	{
		public SpiderProxy(string Host, int Port) : base(Host, Port)
		{
		}
	}
}