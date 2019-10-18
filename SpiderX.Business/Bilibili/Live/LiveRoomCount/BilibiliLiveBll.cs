using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SpiderX.BusinessBase;

namespace SpiderX.Business.Bilibili
{
	public sealed partial class BilibiliLiveBll : BllBase
	{
		public BilibiliLiveBll(ILogger logger, string[] runSetting, int version) : base(logger, runSetting, version)
		{
		}

		public override async Task RunAsync()
		{
			var scheme = new RoomCountScheme() { Collector = new RoomCountPcWebCollector() };
			await scheme.RunAsync().ConfigureAwait(false);
		}
	}
}