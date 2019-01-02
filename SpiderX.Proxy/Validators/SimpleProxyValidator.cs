using System;
using System.Net;
using SpiderX.Http;

namespace SpiderX.Proxy
{
	public sealed class SimpleProxyValidator : SpiderProxyValidator
	{
		public SimpleProxyValidator(string urlString) : base(urlString)
		{
		}

		private static readonly SpiderWebClientPool _clientPool = new SpiderWebClientPool();

		public static readonly SimpleProxyValidator BaiduHttp = new SimpleProxyValidator("http://www.baidu.com")
		{
			Lastword = "</html>",
			Keyword = "baidu"
		};

		public static readonly SimpleProxyValidator BaiduHttps = new SimpleProxyValidator("https://www.baidu.com")
		{
			Lastword = "</html>",
			Keyword = "baidu"
		};

		public string Firstword { get; set; }

		public StringComparison FirstwordComparisonType { get; set; }

		public string Keyword { get; set; }

		public StringComparison KeywordComparisonType { get; set; }

		public string Lastword { get; set; }

		public StringComparison LastwordComparisonType { get; set; }

		public override bool CheckPass(IWebProxy proxy)
		{
			var webClient = _clientPool.Discharge(proxy);
			bool isPassed = false;
			string responseText;
			for (byte i = 0; i < RetryTimes + 1; i++)
			{
				try
				{
					responseText = webClient.GetStringAsync(TargetUri).Result?.Trim();
				}
				catch
				{
					continue;
				}
				if (string.IsNullOrEmpty(responseText))
				{
					continue;
				}
				if (!string.IsNullOrEmpty(Lastword) && !responseText.EndsWith(Lastword, LastwordComparisonType))
				{
					continue;
				}
				if (!string.IsNullOrEmpty(Firstword) && !responseText.StartsWith(Firstword, FirstwordComparisonType))
				{
					continue;
				}
				if (!string.IsNullOrEmpty(Keyword) && !responseText.Contains(Keyword, KeywordComparisonType))
				{
					continue;
				}
				isPassed = true;
				break;
			}
			_clientPool.Recycle(webClient);
			return isPassed;
		}
	}
}