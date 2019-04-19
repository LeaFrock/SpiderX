using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpiderX.Http.Util;
using SpiderX.Proxy;

namespace SpiderX.Business.Bilibili
{
    public partial class BilibiliLiveCountBll
    {
        private abstract class CollectorBase
        {
            public abstract int Collect(string areaId);

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