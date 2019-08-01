using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiderX.Http;
using SpiderX.Http.Util;
using SpiderX.Proxy;

namespace SpiderX.Business.Bilibili
{
	public partial class BilibiliLiveRoomCountBll
	{
		private abstract class CollectorBase
		{
			public HttpRequestCounter RequestCounter { get; protected set; }

			public virtual void BeforeCollectAsync()
			{
				RequestCounter = InternBllHelper.CreateHttpRequestCounter(nameof(BilibiliLiveRoomCountBll), Logger);
			}

			public abstract Task<int> CollectAsync(string areaId);

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