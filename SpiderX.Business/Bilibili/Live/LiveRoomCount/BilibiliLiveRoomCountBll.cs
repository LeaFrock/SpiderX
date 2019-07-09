using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SpiderX.BusinessBase;

namespace SpiderX.Business.Bilibili
{
	public sealed partial class BilibiliLiveRoomCountBll : BllBase
	{
		public BilibiliLiveRoomCountBll(ILogger logger, string[] runSetting, int version) : base(logger, runSetting, version)
		{
		}

		public override async Task RunAsync()
		{
			var scheme = new DefaultScheme() { Collector = new PcWebCollector() };
			scheme.Run();
		}
	}
}