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
using SpiderX.Extensions.Http;
using SpiderX.Http;
using SpiderX.Http.Util;
using SpiderX.Proxy;
using SpiderX.Tools;

namespace SpiderX.Business.LaGou
{
	public sealed partial class LaGouBll
	{
		private class PcWebCollector : CollectorBase
		{
			public bool UseProxy { get; set; }

			public override async Task<LaGouResponseDataCollection> CollectAsync(LaGouSearchParam searchParam)
			{
				string encodedCityName = WebTool.UrlEncodeByW3C(searchParam.CityName);
				string encodedKeyword = WebTool.UrlEncodeByW3C(searchParam.Keyword);
				LaGouResponseDataCollection dataCollection = new LaGouResponseDataCollection();
				using var client = CreateHttpClient();
				await TryInitCookiesAsync(client, encodedCityName, encodedKeyword, searchParam.SearchType).ConfigureAwait(false);
				await Task.Delay(100).ConfigureAwait(false);
				string sid = null;
				await GetResponseData("1").ConfigureAwait(false);
				for (int i = 2; i <= searchParam.MaxPage; i++)
				{
					await Task.Delay(RandomTool.NextIntSafely(4000, 6000)).ConfigureAwait(false);
					await GetResponseData(i.ToString()).ConfigureAwait(false);
				}
				dataCollection.FillPositions(searchParam.Keyword);
				dataCollection.FillCompanies(searchParam.CityName);
				return dataCollection;

				async Task GetResponseData(string pageNum)
				{
					using var postContent = PcWebApiProvider.GetPositionAjaxContent(encodedKeyword, pageNum);
					string ajaxRsp = await PostPositionAjaxAsync(client, postContent, encodedCityName, encodedKeyword, searchParam.SearchType).ConfigureAwait(false);
					var data = PcWebApiProvider.CreateResponseData(ajaxRsp, out sid);
					if (data != null)
					{
						dataCollection.AddResponseData(data);
					}
				}
			}

			private static async Task<bool> TryInitCookiesAsync(SpiderHttpClient client, string encodedCityName, string encodedKeyword, string type = "new")
			{
				var jobListRspMsg = await GetJobListAsync(client, encodedCityName, encodedKeyword, type).ConfigureAwait(false);
				if (jobListRspMsg != null)
				{
					jobListRspMsg.Dispose();
					return true;
				}
				return false;
			}

			private SpiderHttpClient CreateHttpClient()
			{
				var handler = new SocketsHttpHandler()
				{
					AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
					UseCookies = true,
					UseProxy = UseProxy
				};
				if (UseProxy)
				{
					handler.Proxy = HttpConsole.LocalWebProxy;
				}
				SpiderHttpClient client = new SpiderHttpClient(handler);
				var headers = client.DefaultRequestHeaders;
				headers.Host = PcWebApiProvider.HomePageUri.Host;
				headers.Add("Accept-Encoding", "br");
				headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
				headers.Add("User-Agent", HttpConsole.DefaultPcUserAgent);
				return client;
			}

			private static async Task<HttpResponseMessage> GetJobListAsync(SpiderHttpClient client, string encodedCityName, string encodedKeyword, string type = "new")
			{
				Uri uri = PcWebApiProvider.GetJobListUri(encodedCityName, encodedKeyword, type);
				Uri referer = PcWebApiProvider.GetJobListReferer(encodedKeyword);
				using var reqMsg = new HttpRequestMessage(HttpMethod.Get, uri) { Version = HttpVersion.Version11 };
				var headers = reqMsg.Headers;
				headers.Referrer = referer;
				headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3");
				headers.Add("Pragma", "no-cache");
				headers.Add("Cache-Control", "no-cache");
				headers.Add("Upgrade-Insecure-Requests", "1");
				try
				{
					var rspMsg = await client.SendAsync(reqMsg, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
					if (rspMsg.IsSuccessStatusCode)
					{
						FixCookies(client.CookieContainer, uri);
					}
					return rspMsg;
				}
				catch
				{
					return null;
				}
			}

			private static async Task<string> PostPositionAjaxAsync(HttpClient client, HttpContent content, string encodedCityName, string encodedKeyword, string type = "new")
			{
				Uri uri = PcWebApiProvider.GetPositionAjaxUri(encodedCityName, type);
				Uri referer = PcWebApiProvider.GetPostionAjaxReferer(encodedCityName, encodedKeyword, type);
				HttpRequestMessage reqMsg = new HttpRequestMessage(HttpMethod.Post, uri) { Content = content, Version = HttpVersion.Version11 };
				var headers = reqMsg.Headers;
				headers.Referrer = referer;
				headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
				headers.Add("X-Requested-With", "XMLHttpRequest");
				headers.Add("X-Anit-Forge-Code", "0");
				headers.Add("X-Anit-Forge-Token", "None");
				headers.Add("Origin", PcWebApiProvider.HomePageUri.AbsoluteUri);
				HttpResponseMessage rspMsg = null;
				try
				{
					rspMsg = await client.SendAsync(reqMsg).ConfigureAwait(false);
				}
				catch
				{
					rspMsg?.Dispose();
					return null;
				}
				using (rspMsg)
				{
					if (!rspMsg.IsSuccessStatusCode)
					{
						return null;
					}
					return await rspMsg.ToTextAsync().ConfigureAwait(false);
				}
			}

			private static void FixCookies(CookieContainer container, Uri uri)
			{
				var cc = container.GetCookies(uri);
				for (int i = 0; i < cc.Count; i++)
				{
					Cookie cookie = cc[i];
					if (cookie.Version == 1)
					{
						Cookie fixedCookie = new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain)
						{
							Expires = cookie.Expires,
							Expired = false
						};
						container.Add(fixedCookie);
						cc[i].Expired = true;
					}
				}
			}
		}
	}
}