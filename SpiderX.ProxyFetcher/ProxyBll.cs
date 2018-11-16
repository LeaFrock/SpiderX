using System;
using SpiderX.BusinessBase;
using SpiderX.DataClient;
using SpiderX.Proxy;
using SpiderX.Tools;

namespace SpiderX.ProxyFetcher
{
	public abstract class ProxyBll : BllBase
	{
		protected static Random RandomEvent => CommonTool.RandomEvent;

		internal abstract ProxyApiProvider ApiProvider { get; }

		public static ProxyAgent CreateProxyAgent()
		{
			var conf = DbClient.Default.FindConfig("SqlServerTest", true);
			if (conf == null)
			{
				throw new DbConfigNotFoundException();
			}
			return new ProxyAgent(conf);
		}
	}
}