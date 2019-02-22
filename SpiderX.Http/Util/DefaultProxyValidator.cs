using System;
using System.Net.Http;
using System.Threading.Tasks;
using SpiderX.Proxy;

namespace SpiderX.Http.Util
{
	public sealed class DefaultProxyValidator : ProxyValidatorBase
	{
		public DefaultProxyValidator(string urlString) : base(urlString)
		{
		}

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

		public override bool CheckPass(HttpResponseMessage response)
		{
			return base.CheckPass(response);
		}

		private bool CheckResponseTask(Task<string> responseTask)
		{
			string text = responseTask.ConfigureAwait(false).GetAwaiter().GetResult();
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