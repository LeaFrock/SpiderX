using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SpiderX.BusinessBase;

namespace SpiderX.Business.Bilibili
{
	public sealed partial class BilibiliLiveRoomCountBll : BllBase
	{
		public override async Task RunAsync()
		{
			await base.RunAsync();
			var scheme = new DefaultScheme() { Collector = new PcWebCollector() };
			scheme.Run();
		}

		public override Task RunAsync(params string[] args)
		{
			return RunAsync();
		}
	}
}