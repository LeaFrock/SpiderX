using System;
using System.Net;
using SpiderX.Proxy;

namespace SpiderX.Http.Util
{
	public sealed class DefaultProxyValidator : ProxyValidatorBase
	{
		public DefaultProxyValidator(string urlString) : base(urlString)
		{
		}

		private static readonly SpiderWebClientPool _clientPool = new SpiderWebClientPool();

		public static readonly DefaultProxyValidator BaiduHttp = new DefaultProxyValidator("http://www.baidu.com")
		{
			Lastword = "</html>",
			Keyword = "baidu",
			KeywordComparisonType = StringComparison.CurrentCultureIgnoreCase
		};

		public static readonly DefaultProxyValidator BaiduHttps = new DefaultProxyValidator("https://www.baidu.com")
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

		public override bool CheckPass(IWebProxy proxy)
		{
			var webClient = _clientPool.Distribute(proxy);
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