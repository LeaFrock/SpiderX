using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SpiderX.Business.LaGou.DbContexts;

namespace SpiderX.Business.LaGou
{
	public sealed partial class LaGouBll
	{
		private class DefaultScheme : SchemeBase
		{
			public override async Task RunAsync(LaGouSearchParam searchParam)
			{
				var datas = await Collector.CollectAsync(searchParam);
				using (var context = new LaGouSqlServerContext())
				{
					context.Database.EnsureCreated();
					context.Positions.AddRange(datas.Positions.Values);
					context.Companies.AddRange(datas.Companies.Values);
					context.HrInfos.AddRange(datas.HrInfos.Values);
					context.HrDailyRecords.AddRange(datas.HrDailyRecords.Values);
					context.SaveChanges();
				}
			}
		}
	}
}