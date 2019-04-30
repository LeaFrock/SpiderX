using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderX.Business.Bilibili
{
    public partial class BilibiliLiveRoomCountBll
    {
        private sealed class DefaultScheme : SchemeBase
        {
            public override void Run()
            {
                int liveRoomCount = Collector.Collect("0");
                ShowConsoleMsg(liveRoomCount.ToString());
                using (var context = new BilibiliLiveRoomCountMySqlContext())
                {
                    context.Database.EnsureCreated();
                    var item = BilibiliLiveRoomCount.Create(liveRoomCount);
                    context.LiveRoomCount.Add(item);
                    context.SaveChanges();
                }
            }
        }
    }
}