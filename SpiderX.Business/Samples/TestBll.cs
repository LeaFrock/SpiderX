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
using SpiderX.Tools;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;

namespace SpiderX.Business.Samples
{
	public sealed class TestBll : BllBase
	{
		public TestBll(ILogger logger, string[] runSetting, int version) : base(logger, runSetting, version)
		{
		}

		public event EventHandler Eeee;

		public override async Task RunAsync()
		{
			var conf = DbConfigManager.Default.GetConfig("SqlServerTest", true);
			if (conf == null)
			{
				throw new DbConfigNotFoundException();
			}
			var eventOpt = new SpiderProxyServerEventOption() { AfterResponse = AfterResponse };
			var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 8000, true);
			var proxyServer = SpiderProxyServer.StartNew(explicitEndPoint, eventOpt);
			proxyServer.Close();
			await Task.CompletedTask;
		}

		public async Task AfterResponse(object sender, SessionEventArgs args)
		{
			await Task.CompletedTask;
		}
	}
}