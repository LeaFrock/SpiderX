using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SpiderX.BusinessBase;

namespace SpiderX.Business.TenderWebs
{
	public partial class GgzyGovBll
	{
		public sealed class DefaultScheme : SchemeBase
		{
			public override CollectorBase Collector { get; } = new PcWebCollector();

			public override async Task RunAsync(params string[] keywords)
			{
				await Collector.CollectOpenBids(keywords);
			}
		}
	}
}