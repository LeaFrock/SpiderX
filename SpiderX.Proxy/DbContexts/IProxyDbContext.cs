using SpiderX.DataClient;

namespace SpiderX.Proxy
{
	internal interface IProxyDbContext
	{
		DbConfig Config { get; }
	}
}