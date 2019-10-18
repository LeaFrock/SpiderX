using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SpiderX.Http;

namespace SpiderX.Business.Bilibili
{
	public partial class BilibiliLiveBll
	{
		private abstract class SchemeBase
		{
			public CollectorBase Collector { get; set; }

			public abstract Task RunAsync();
		}
	}
}