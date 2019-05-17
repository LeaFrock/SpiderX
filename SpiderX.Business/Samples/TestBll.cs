using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SpiderX.BusinessBase;
using SpiderX.DataClient;
using SpiderX.Http;
using SpiderX.Http.Util;
using SpiderX.Proxy;

namespace SpiderX.Business.Samples
{
	public sealed class TestBll : BllBase
	{
		public override void Run(params string[] args)
		{
			Run();
		}

		public override void Run()
		{
			var conf = DbClient.FindConfig("SqlServerTest", true);
			if (conf == null)
			{
				throw new DbConfigNotFoundException();
			}
			var proxyAgent = ProxyAgent<SqlServerProxyDbContext>.CreateInstance(conf, c => new SqlServerProxyDbContext(c));
			DefaultProxyUriLoader proxyUriLoader = new DefaultProxyUriLoader()
			{
				Days = 360,
				Condition = p => p.Id > 0,
				ProxyAgent = proxyAgent
			};
			DefaultWebProxySelector proxySelector = new DefaultWebProxySelector(new Uri("http://www.baidu.com"), proxyUriLoader, CreateWebClient, ValidateWebProxy)
			{
				UseThresold = 1,
				VerifyPauseThresold = 2
			};
		}

		public static SpiderWebClient CreateWebClient(IWebProxy proxy)
		{
			SpiderWebClient client = new SpiderWebClient(proxy);
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