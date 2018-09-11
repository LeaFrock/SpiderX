using SpiderX.BusinessBase;

namespace SpiderX.ProxyFetcher
{
	public class IpHaiProxyBll : BllBase
	{
		public const string Url = "";

		public override string ClassName => GetType().Name;
		
		public override void Run(params string[] objs)
		{
			throw new System.NotImplementedException();
		}
	}
}