using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SpiderX.BusinessBase;
using SpiderX.DataClient;

namespace SpiderX.Business.Bilibili
{
	public sealed partial class BilibiliLiveBll : BllBase
	{
		public BilibiliLiveBll(ILogger logger, string[] runSetting, string dbConfigName, int version) : base(logger, runSetting, dbConfigName, version)
		{
		}

		public override async Task RunAsync()
		{
			var dbConf = DbConfigManager.Default.GetConfig(DbConfigName) ?? throw new DbConfigNotFoundException($"Config '{DbConfigName}' not found.");
			var scheme = new RoomCountScheme()
			{
				DbConfig = dbConf,
				Collector = new RoomCountPcWebCollector()
			};
			await scheme.RunAsync().ConfigureAwait(false);
		}
	}
}