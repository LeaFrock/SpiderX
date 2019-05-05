using System;
using System.Collections.Generic;
using System.Text;
using SpiderX.BusinessBase;

namespace SpiderX.Business.TenderWebs
{
    public sealed partial class GgzyGovBll : BllBase
    {
        public abstract class CollectorBase
        {
            public abstract List<OpenTenderEntity> CollectOpenBids(string keyword);

            public abstract List<WinTenderEntity> CollectWinBids(string keyword);
        }
    }
}