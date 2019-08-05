using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp;
using SpiderX.Extensions.Http;
using SpiderX.Http;
using SpiderX.Http.Util;
using SpiderX.Proxy;
using SpiderX.Puppeteer;
using SpiderX.Tools;

namespace SpiderX.Business.LaGou
{
	public sealed partial class LaGouBll
	{
		private class PcWebPptrCollector : CollectorBase
		{
			private readonly LaGouResponseDataCollection dataCollection = new LaGouResponseDataCollection();

			public override async Task<LaGouResponseDataCollection> CollectAsync(LaGouSearchParam searchParam)
			{
				string encodedCityName = WebTool.UrlEncodeByW3C(searchParam.CityName);
				string encodedKeyword = WebTool.UrlEncodeByW3C(searchParam.Keyword);
				using (var browser = await PuppeteerConsole.LauncherBrowser(false))
				{
					using (var page = await browser.NewPageAsync())
					{
						page.Response += OnResponsed;
						var uri = PcWebApiProvider.GetJobListUri(encodedCityName, encodedKeyword, searchParam.SearchType);
						await page.GoToAsync(uri.AbsoluteUri, WaitUntilNavigation.Networkidle2);
						await Task.Delay(1000);
						dataCollection.FillPositions(searchParam.Keyword);
						dataCollection.FillCompanies(searchParam.CityName);
						return dataCollection;
					}
				}
			}

			private void OnResponsed(object sender, ResponseCreatedEventArgs args)
			{
				var rsp = args.Response;
				if (!rsp.Url.StartsWith("https://www.lagou.com/jobs/positionAjax.json"))
				{
					return;
				}
				if (!rsp.Ok)
				{
					return;
				}
				string ajaxRsp = rsp.TextAsync().Result;
				var data = PcWebApiProvider.CreateResponseData(ajaxRsp, out string _);
				if (data != null)
				{
					dataCollection.AddResponseData(data);
				}
			}
		}
	}
}