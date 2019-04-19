using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpiderX.Http.Util;
using SpiderX.Proxy;

namespace SpiderX.Business.LaGou
{
    public sealed partial class LaGouBll
    {
        private abstract class CollectorBase
        {
            public abstract LaGouResponseDataCollection Collect(string cityName, string keyword);

            protected virtual IProxyUriLoader CreateProxyUriLoader()
            {
                var proxyAgent = ProxyAgent<SqlServerProxyDbContext>.CreateInstance("SqlServerTest", true, c => new SqlServerProxyDbContext(c));
                DefaultProxyUriLoader loader = new DefaultProxyUriLoader()
                {
                    Days = 360,
                    Condition = e => e.Category == 1 && e.AnonymityDegree == 3
                };
                return loader;
            }
        }
    }
}