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
                EnsureDbCreated(() => new BilibiliLiveRoomCountMySqlContext());
                int liveRoomCount = Collector.Collect("0");
                ShowConsoleMsg(liveRoomCount.ToString());
                var item = BilibiliLiveRoomCount.Create(liveRoomCount);
                using (var context = new BilibiliLiveRoomCountMySqlContext())
                {
                    context.LiveRoomCount.Add(item);
                    context.SaveChanges();
                }
            }

            private static void EnsureDbCreated<T>(Func<T> contextFactory) where T : BilibiliLiveRoomCountContext
            {
                using (var context = contextFactory.Invoke())
                {
                    context.Database.EnsureCreated();
                }
            }
        }
    }
}