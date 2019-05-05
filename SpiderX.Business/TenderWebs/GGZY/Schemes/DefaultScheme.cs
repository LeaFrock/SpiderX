using System;
using System.Collections.Generic;
using System.Text;
using SpiderX.BusinessBase;

namespace SpiderX.Business.TenderWebs
{
    public partial class GgzyGovBll
    {
        public sealed class DefaultScheme : SchemeBase
        {
            public override CollectorBase Collector { get; } = new PcWebCollector();

            public override void Run(params string[] keywords)
            {
                throw new NotImplementedException();
            }
        }
    }
}