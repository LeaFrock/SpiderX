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
	public partial class BilibiliLiveBll
	{
		private abstract class CollectorBase
		{
			public abstract Task<int> CollectRoomCountAsync(string areaId);
		}
	}
}