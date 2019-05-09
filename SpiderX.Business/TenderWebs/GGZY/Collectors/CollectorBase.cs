using System;
using System.Collections.Generic;
using System.Text;
using SpiderX.BusinessBase;
using SpiderX.Http.Util;
using SpiderX.Proxy;

namespace SpiderX.Business.TenderWebs
{
    public partial class GgzyGovBll
    {
        public abstract class CollectorBase
        {
            public abstract List<OpenTenderEntity> CollectOpenBids(string[] keywords);

            public abstract List<WinTenderEntity> CollectWinBids(string[] keywords);

            protected virtual IProxyUriLoader CreateProxyUriLoader()
            {
                var proxyAgent = ProxyAgent<SqlServerProxyDbContext>.CreateInstance("SqlServerTest", true, c => new SqlServerProxyDbContext(c));
                DefaultProxyUriLoader loader = new DefaultProxyUriLoader()
                {
                    ProxyAgent = proxyAgent,
                    Days = 360,
                    Condition = e => e.Category == 1 && e.AnonymityDegree == 3
                };
                return loader;
            }
        }
    }
}