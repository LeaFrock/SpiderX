using System;
using SpiderX.BusinessBase;
using SpiderX.Extensions;
using SpiderX.Tools;

namespace SpiderX.Business.LaGou
{
	public sealed partial class LaGouBll : BllBase
	{
		private CollectorBase _collector = new PcWebCollector();

		public override void Run()
		{
			_collector.Collect("上海", ".NET");
		}

		public override void Run(params string[] args)
		{
			Run();
		}
	}
}