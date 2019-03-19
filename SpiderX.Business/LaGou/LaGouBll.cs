using System;
using SpiderX.Business.LaGou.DbContexts;
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
			var datas = _collector.Collect("上海", ".NET");
			using (var context = new LaGouSqlServerContext())
			{
				context.Positions.AddRange(datas.Positions.Values);
				context.Companies.AddRange(datas.Companies.Values);
				context.HrInfos.AddRange(datas.HrInfos.Values);
				context.HrDailyRecords.AddRange(datas.HrDailyRecords.Values);
				context.SaveChanges();
			}
		}

		public override void Run(params string[] args)
		{
			Run();
		}
	}
}