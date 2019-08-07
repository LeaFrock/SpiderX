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
			private const string NextPageElementSelector = "div[class=pager_container] span[class*=next]";

			public override async Task<LaGouResponseDataCollection> CollectAsync(LaGouSearchParam searchParam)
			{
				string encodedCityName = WebTool.UrlEncodeByW3C(searchParam.CityName);
				string encodedKeyword = WebTool.UrlEncodeByW3C(searchParam.Keyword);
				var jobListUri = PcWebApiProvider.GetJobListUri(encodedCityName, encodedKeyword, searchParam.SearchType);
				LaGouResponseDataCollection dataCollection = new LaGouResponseDataCollection();
				using (var browser = await PuppeteerConsole.LauncherBrowser(false))
				{
					using (var page = await browser.NewPageAsync())
					{
						page.Response += OnResponsed;
						await page.GoToAsync(jobListUri.AbsoluteUri);//Get the first page directly.
						for (int i = 0; i < searchParam.MaxPage - 1; i++)
						{
							await Task.Delay(RandomTool.NextInt(2000, 4000));
							await page.HoverAsync(NextPageElementSelector);
							await Task.Delay(RandomTool.NextInt(3000, 5000));
							await page.ClickAsync(NextPageElementSelector);
						}
						return dataCollection;
					}
				}

				async void OnResponsed(object sender, ResponseCreatedEventArgs args)
				{
					var rsp = args.Response;
					if (!rsp.Url.StartsWith(PcWebApiProvider.PositionAjaxUrlPrefix))
					{
						return;
					}
					if (!rsp.Ok)
					{
						return;
					}
					string rspText = null;
					try
					{
						rspText = await rsp.TextAsync();
					}
					catch (Exception ex)
					{
						ShowLogException(ex);
						return;
					}
					var data = PcWebApiProvider.CreateResponseData(rspText, out string _);
					if (data is null)
					{
						return;
					}
					dataCollection.AddResponseData(data);
					dataCollection.FillPositions(searchParam.Keyword);
					dataCollection.FillCompanies(searchParam.CityName);
				}
			}
		}
	}
}