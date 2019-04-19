using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderX.Business.Bilibili
{
    public partial class BilibiliLiveCountBll
    {
        private sealed class DefaultScheme : SchemeBase
        {
            public override void Run()
            {
                int liveRoomCount = Collector.Collect("0");
                ShowConsoleMsg(liveRoomCount.ToString());
            }
        }
    }
}