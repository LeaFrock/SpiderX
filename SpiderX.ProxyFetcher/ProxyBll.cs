using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SpiderX.BusinessBase;
using SpiderX.Extensions;
using SpiderX.Extensions.Http;
using SpiderX.Http;
using SpiderX.Proxy;
using SpiderX.Tools;

namespace SpiderX.ProxyFetcher
{
	public abstract class ProxyBll : BllBase
	{
		protected static Random RandomEvent => CommonTool.RandomEvent;

		internal abstract ProxyApiProvider ApiProvider { get; }

		protected List<SpiderProxyUriEntity> GetProxyEntities(SpiderWebClient webClient, HttpMethod httpMethod, string url)
		{
			List<SpiderProxyUriEntity> entities = null;
			Task task = webClient.SendAsync(httpMethod, url)
				.ContinueWith(requestTask =>
				{
					HttpResponseMessage responseMessage = requestTask.Result;
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
						var entityTask = responseMessage.Content.ToHtmlReaderAsync()
						.ContinueWith(t =>
						{
							StreamReader reader = t.Result;
							entities = ApiProvider.GetProxyEntities(reader);
						}, TaskContinuationOptions.OnlyOnRanToCompletion);
						try
						{
							entityTask.Wait(10000);
						}
						catch
						{
						}
					}
				}, TaskContinuationOptions.OnlyOnRanToCompletion);
			try
			{
				task.Wait();
			}
			catch
			{
			}
			return entities ?? new List<SpiderProxyUriEntity>(0);
		}

		protected List<SpiderProxyUriEntity> GetProxyEntities(SpiderWebClient webClient, HttpMethod httpMethod, IList<string> urls, int estimatedCount = 0)
		{
			var entities = estimatedCount > 0 ? new List<SpiderProxyUriEntity>(estimatedCount) : new List<SpiderProxyUriEntity>();
			int urlCount = urls.Count;
			Task[] tasks = new Task[urlCount];
			for (int i = 0; i < urlCount; i++)
			{
				tasks[i] = webClient.SendAsync(httpMethod, urls[i])
					.ContinueWith(requestTask =>
				   {
					   HttpResponseMessage responseMessage = requestTask.Result;
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
	}
}