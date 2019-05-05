using System;
using System.Collections.Generic;
using System.Text;
using SpiderX.BusinessBase;

namespace SpiderX.Business.TenderWebs
{
    public sealed partial class GgzyGovBll : BllBase
    {
        public sealed class PcWebCollector : CollectorBase
        {
            public override List<OpenTenderEntity> CollectOpenBids(string keyword)
            {
                return null;
            }

            public override List<WinTenderEntity> CollectWinBids(string keyword)
            {
                return null;
            }
        }
    }
}