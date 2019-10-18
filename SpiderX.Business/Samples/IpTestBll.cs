using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SpiderX.BusinessBase;
using SpiderX.DataClient;
using SpiderX.Http;
using SpiderX.Http.Util;
using SpiderX.Proxy;
using SpiderX.Tools;

namespace SpiderX.Business.Samples
{
	public sealed class IpTestBll : BllBase
	{
		public const string HomePageHost = "icanhazip.com";
		private readonly Uri HomePageUri = new Uri("http://icanhazip.com");

		public IpTestBll(ILogger logger, string[] runSetting, string dbConfigName, int version) : base(logger, runSetting, dbConfigName, version)
		{
		}

		public override async Task RunAsync()
		{
			DefaultProxyUriLoader proxyUriLoader = new DefaultProxyUriLoader()
			{
				Days = 360,
				Condition = p => p.Id > 0,
				DbContextFactory = () => ProxyDbContext.CreateInstance("SqlServerTest")
			};
			DefaultWebProxyValidator webProxyValidator = new DefaultWebProxyValidator(CreateWebClient, ValidateWebProxy, new WebProxyValidatorConfig()
			{
				UseThresold = 1,
				VerifyPauseThresold = 2
			});
			DefaultWebProxySelector proxySelector = new DefaultWebProxySelector(new Uri("http://www.baidu.com"), proxyUriLoader, webProxyValidator);
			proxySelector.Initialize();
			string rspText = await HttpConsole.GetResponseTextByProxyAsync(HomePageUri, proxySelector, GetResponseTextAsync).ConfigureAwait(false);
			ShowLogInfo(rspText);
		}

		public static async Task<string> GetResponseTextAsync(Uri uri, IWebProxy proxy)
		{
			using var client = CreateWebClient(proxy);
			try
			{
				return await client.GetStringAsync(uri).ConfigureAwait(false);
			}
			catch
			{
				return string.Empty;
			}
		}

		public static SpiderHttpClient CreateWebClient(IWebProxy proxy)
		{
			SpiderHttpClient client = new SpiderHttpClient(proxy);
			client.DefaultRequestHeaders.Host = HomePageHost;
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
			return rspText.EndsWith("</html>") && rspText.Contains("baidu", StringComparison.OrdinalIgnoreCase);
		}
	}
}