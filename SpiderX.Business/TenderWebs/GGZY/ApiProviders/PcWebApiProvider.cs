using System;
using System.Collections.Generic;
using System.Text;
using SpiderX.BusinessBase;

namespace SpiderX.Business.TenderWebs
{
    public sealed partial class GgzyGovBll : BllBase
    {
        public sealed class PcWebApiProvider : ApiProviderBase
        {
            public readonly static Uri ApiUri_GetBids = new Uri("http://deal.ggzy.gov.cn/ds/deal/dealList_find.jsp");
        }
    }
}