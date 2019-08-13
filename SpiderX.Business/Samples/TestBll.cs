using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SpiderX.BusinessBase;
using SpiderX.DataClient;
using SpiderX.Redis;
using StackExchange.Redis;

namespace SpiderX.Business.Samples
{
	public sealed class TestBll : BllBase
	{
		public TestBll(ILogger logger, string[] runSetting, int version) : base(logger, runSetting, version)
		{
		}

		public override async Task RunAsync()
		{
			var conf = DbConfigManager.Default.GetConfig("SqlServerTest", true);
			if (conf == null)
			{
				throw new DbConfigNotFoundException();
			}
			var confOpts = ConfigurationOptions.Parse("localhost:6379");
			var client = await SpiderRedisClient.CreateAsync(confOpts);
		}
	}
}