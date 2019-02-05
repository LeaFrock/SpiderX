using System;
using System.Collections.Generic;
using System.Text;
using SpiderX.BusinessBase;
using SpiderX.Http;

namespace SpiderX.Business.Samples
{
	public sealed class IpTestBll : BllBase
	{
		public const string HomePageHost = "icanhazip.com";
		public const string HomePageUrl = "http://icanhazip.com";

		public override void Run()
		{
			base.Run();
			using (var client = CreateWebClient())
			{
				string r = client.GetStringAsync(HomePageUrl).ConfigureAwait(false).GetAwaiter().GetResult();
				ShowConsoleMsg(r);
			}
		}

		public override void Run(params string[] args)
		{
			Run();
		}

		public static SpiderWebClient CreateWebClient()
		{
			SpiderWebClient client = SpiderWebClient.CreateDefault();
			client.InnerClientHandler.UseProxy = false;
			client.DefaultRequestHeaders.Host = HomePageHost;
			client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
			client.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
			client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
			client.DefaultRequestHeaders.Add("User-Agent", HttpConsole.DefaultPcUserAgent);
			return client;
		}
	}
}