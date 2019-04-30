using System;
using System.Collections.Generic;
using System.Text;
using SpiderX.BusinessBase;

namespace SpiderX.Business.Bilibili
{
    public sealed partial class BilibiliLiveRoomCountBll : BllBase
    {
        public override void Run()
        {
            base.Run();
            var scheme = new DefaultScheme() { Collector = new PcWebCollector() };
            scheme.Run();
        }

        public override void Run(params string[] args)
        {
            Run();
        }
    }
}