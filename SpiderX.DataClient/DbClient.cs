using Microsoft.Extensions.Configuration;

namespace SpiderX.DataClient
{
	public static class DbClient
	{
		internal static DbClientSetting Setting { get; private set; }

		public static DbConfig FindConfig(string name, bool isTest)
		{
			return Setting.FindConfig(name, isTest);
		}

		public static void Initialize(IConfigurationRoot root)
		{
			Setting = DbClientSetting.CreateInstance(root);
		}
	}
}