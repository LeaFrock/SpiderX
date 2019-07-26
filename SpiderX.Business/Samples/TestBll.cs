using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpiderX.BusinessBase;
using SpiderX.DataClient;
using SpiderX.Http;
using SpiderX.Http.Util;
using SpiderX.Proxy;
using SpiderX.Puppeteer;

namespace SpiderX.Business.Samples
{
	public sealed class TestBll : BllBase
	{
		public TestBll(ILogger logger, string[] runSetting, int version) : base(logger, runSetting, version)
		{
		}

		public override async Task RunAsync()
		{
			var conf = DbConfigManager.Default.GetConfig("SqlServerTest", true);
			if (conf == null)
			{
				throw new DbConfigNotFoundException();
			}
			//var proxyAgent = ProxyAgent<SqlServerProxyDbContext>.CreateInstance(conf, c => new SqlServerProxyDbContext(c));
			//DefaultProxyUriLoader proxyUriLoader = new DefaultProxyUriLoader()
			//{
			//	Days = 360,
			//	Condition = p => p.Id > 0,
			//	ProxyAgent = proxyAgent
			//};
			//DefaultWebProxySelector proxySelector = new DefaultWebProxySelector(new Uri("http://www.baidu.com"), proxyUriLoader, CreateWebClient, ValidateWebProxy)
			//{
			//	UseThresold = 1,
			//	VerifyPauseThresold = 2
			//};
			for (int i = 0; i < 2; i++)
			{
				var browser = await PuppeteerConsole.LauncherBrowser(false);
				var page = await browser.NewPageAsync();
				await page.GoToAsync("https://www.baidu.com");
				Console.ReadKey();
			}
			PuppeteerConsole.CloseAllBrowsers();
		}

		public static SpiderHttpClient CreateWebClient(IWebProxy proxy)
		{
			SpiderHttpClient client = new SpiderHttpClient(proxy);
			client.DefaultRequestHeaders.Host = "www.baidu.com";
			client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
			client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
			client.DefaultRequestHeaders.Add("Pragma", "no-cache");
			client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
			client.DefaultRequestHeaders.Add("User-Agent", HttpConsole.DefaultPcUserAgent);
			return client;
		}

		private static bool ValidateWebProxy(string rspText)
		{
			return rspText.EndsWith("</html>") && rspText.Contains("baidu");
		}
	}
}