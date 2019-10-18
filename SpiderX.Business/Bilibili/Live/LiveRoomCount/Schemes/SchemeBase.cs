using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SpiderX.DataClient;
using SpiderX.Http;

namespace SpiderX.Business.Bilibili
{
	public partial class BilibiliLiveBll
	{
		private abstract class SchemeBase
		{
			public DbConfig DbConfig { get; set; }

			public CollectorBase Collector { get; set; }

			public abstract Task RunAsync();
		}
	}
}