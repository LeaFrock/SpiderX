using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderX.ProxyFetcher
{
	internal class KxDailiProxyBll : ProxyBll
	{
		internal override ProxyApiProvider ApiProvider { get; } = new KxDailiProxyApiProvider();

		public override void Run(params string[] args)
		{
			throw new NotImplementedException();
		}
	}
}
