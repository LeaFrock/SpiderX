using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SpiderX.BusinessBase;
using SpiderX.Http.Util;
using SpiderX.Proxy;

namespace SpiderX.Business.TenderWebs
{
	public partial class GgzyGovBll
	{
		private abstract class CollectorBase
		{
			public abstract Task<List<OpenTenderEntity>> CollectOpenBids(string[] keywords);

			public abstract Task<List<WinTenderEntity>> CollectWinBids(string[] keywords);

			protected virtual IProxyUriLoader CreateProxyUriLoader()
			{
				DefaultProxyUriLoader loader = new DefaultProxyUriLoader()
				{
					DbContextFactory = () => ProxyDbContext.CreateInstance(),
					Days = 360,
					EntityOption = new SpiderProxyUriEntityOption() { Category = 1, AnonymityDegree = 3, ResponseMilliseconds = 10000 }
				};
				return loader;
			}
		}
	}
}