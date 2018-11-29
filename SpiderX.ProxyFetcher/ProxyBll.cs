using System;
using SpiderX.BusinessBase;
using SpiderX.Tools;

namespace SpiderX.ProxyFetcher
{
	public abstract class ProxyBll : BllBase
	{
		protected static Random RandomEvent => CommonTool.RandomEvent;

		internal abstract ProxyApiProvider ApiProvider { get; }
	}
}