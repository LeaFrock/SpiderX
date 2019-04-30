using System;
using System.Collections.Generic;
using System.Text;
using SpiderX.Business.LaGou.DbContexts;

namespace SpiderX.Business.LaGou
{
    public sealed partial class LaGouBll
    {
        private class DefaultScheme : SchemeBase
        {
            public override void Run()
            {
                var datas = Collector.Collect("上海", ".NET");
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