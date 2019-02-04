using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SpiderX.Extensions;
using SpiderX.Extensions.Http;
using SpiderX.Http;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher
{
	public class YunDailiProxyBll : ProxyBll
	{
		internal override ProxyApiProvider ApiProvider { get; } = new YunDailiProxyApiProvider();

		public override void Run()
		{
			base.Run();
			string caseName = ClassName;
			var pa = ProxyAgent<SqlServerProxyDbContext>.CreateInstance("SqlServerTest", true, c => new SqlServerProxyDbContext(c));
			var urls = ApiProvider.GetRequestUrls();
			using (SpiderWebClient webClient = ApiProvider.CreateWebClient())
			{
				var entities = GetProxyEntities(webClient, HttpMethod.Get, urls, 200);
				if (entities.Count < 1)
				{
					return;
				}
				entities.ForEach(e => e.Source = caseName);
				ShowConsoleMsg("CollectCount: " + entities.Count.ToString());
				int insertCount = pa.InsertProxyEntities(entities);
				ShowConsoleMsg("InsertCount: " + insertCount.ToString());
			}
		}

		public override void Run(params string[] args)
		{
			Run();
		}

		private List<SpiderProxyEntity> GetProxyEntities(SpiderWebClient webClient, string urlTemplate, int maxPage)
		{
			List<SpiderProxyEntity> entities = new List<SpiderProxyEntity>(100);
			Task[] tasks = new Task[maxPage];
			for (int p = 1; p <= maxPage; p++)
			{
				string url = GetRequestUrl(urlTemplate, p);
				tasks[p - 1] = GetResponseMessageAsync(webClient, url)
					.ContinueWith(responseMsg =>
					{
						HttpResponseMessage responseMessage = responseMsg.Result;
						if (responseMessage == null)
						{
							return;
						}
						using (responseMessage)
						{
							if (!responseMessage.IsSuccessStatusCode)
							{
								return;
							}
							responseMessage.Content.ToHtmlReaderAsync()
							.ContinueWith(t =>
							{
								StreamReader reader = t.Result;
								var tempList = ApiProvider.GetProxyEntities(reader);
								if (!tempList.IsNullOrEmpty())
								{
									lock (entities)
									{
										entities.AddRange(tempList);
									}
								}
							}, TaskContinuationOptions.OnlyOnRanToCompletion);
						}
					}, TaskContinuationOptions.OnlyOnRanToCompletion);
				Thread.Sleep(RandomEvent.Next(4000, 6000));
			}
			try
			{
				Task.WaitAll(tasks);
			}
			catch
			{
			}
			return entities;
		}

		private static async Task<HttpResponseMessage> GetResponseMessageAsync(SpiderWebClient webClient, string requestUrl)
		{
			var request = CreateRequestMessage(requestUrl);
			try
			{
				return await webClient.SendAsync(request);
			}
			catch (Exception)
			{
				request.Dispose();
				return null;
			}
		}

		private static HttpRequestMessage CreateRequestMessage(string requestUrl)
		{
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
			return request;
		}

		private static string GetRequestUrl(string urlTemplate, int page) => urlTemplate + page.ToString();
	}
}