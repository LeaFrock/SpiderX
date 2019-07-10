using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpiderX.Business.Bilibili
{
	public partial class BilibiliLiveRoomCountBll
	{
		private abstract class SchemeBase
		{
			public CollectorBase Collector { get; set; }

			public abstract Task RunAsync();
		}
	}
}