using System;
using System.Net.Http;
using System.Threading.Tasks;
using SpiderX.Proxy;

namespace SpiderX.Http.Util
{
	public sealed class DefaultProxyUriValidator : ProxyUriValidatorBase
	{
		public DefaultProxyUriValidator(string urlString) : base(urlString)
		{
		}

		private static readonly Lazy<SpiderWebClientPool> _clientPoolLazy = new Lazy<SpiderWebClientPool>();

		internal static SpiderWebClientPool ClientPool => _clientPoolLazy.Value;

		public static readonly DefaultProxyUriValidator BaiduHttp = new DefaultProxyUriValidator("http://www.baidu.com")
		{
			Lastword = "</html>",
			Keyword = "baidu",
			KeywordComparisonType = StringComparison.CurrentCultureIgnoreCase
		};

		public static readonly DefaultProxyUriValidator BaiduHttps = new DefaultProxyUriValidator("https://www.baidu.com")
		{
			Lastword = "</html>",
			Keyword = "baidu",
			KeywordComparisonType = StringComparison.CurrentCultureIgnoreCase
		};

		public string Firstword { get; set; }

		public StringComparison FirstwordComparisonType { get; set; }

		public string Keyword { get; set; }

		public StringComparison KeywordComparisonType { get; set; }

		public string Lastword { get; set; }

		public StringComparison LastwordComparisonType { get; set; }

		public override bool CheckPass(Uri proxy)
		{
			var webClient = ClientPool.Distribute(proxy);
			bool isPassed = false;
			for (byte i = 0; i < RetryTimes + 1; i++)
			{
				if (CheckResponse(webClient))
				{
					isPassed = true;
					break;
				}
			}
			ClientPool.Recycle(webClient);
			return isPassed;
		}

		private bool CheckResponse(HttpClient client)
		{
			Task<string> responseTask = client.GetStringAsync(TargetUri);
			Task<bool> readTask = responseTask.ContinueWith(CheckResponseTask, TaskContinuationOptions.OnlyOnRanToCompletion);
			try
			{
				readTask.Wait();
			}
			catch
			{
				return false;
			}
			return readTask.Result;
		}

		private bool CheckResponseTask(Task<string> responseTask)
		{
			string text = responseTask.Result?.Trim();
			return CheckResponseText(text);
		}

		private bool CheckResponseText(string responseText)
		{
			if (string.IsNullOrEmpty(responseText))
			{
				return false;
			}
			if (!string.IsNullOrEmpty(Lastword) && !responseText.EndsWith(Lastword, LastwordComparisonType))
			{
				return false;
			}
			if (!string.IsNullOrEmpty(Firstword) && !responseText.StartsWith(Firstword, FirstwordComparisonType))
			{
				return false;
			}
			if (!string.IsNullOrEmpty(Keyword) && !responseText.Contains(Keyword, KeywordComparisonType))
			{
				return false;
			}
			return true;
		}
	}
}