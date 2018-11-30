using System;
using System.Collections.Generic;
using System.Text;
using SpiderX.BusinessBase;
using SpiderX.DataClient;
using SpiderX.Proxy;

namespace SpiderX.Business
{
	public sealed class TestBll : BllBase
	{
		public override void Run(params string[] args)
		{
			Run();
		}

		public override void Run()
		{
			var conf = DbClient.FindConfig("SqlServerTest", true);
			if (conf == null)
			{
				throw new DbConfigNotFoundException();
			}
			var pa = ProxyAgent<SqlServerProxyDbContext>.CreateInstance(conf, c => new SqlServerProxyDbContext(c));
			var entities = pa.SelectProxyEntities(p => p.Category == 1);
		}
	}
}