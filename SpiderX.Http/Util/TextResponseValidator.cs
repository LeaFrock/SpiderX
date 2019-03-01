using System;

namespace SpiderX.Http.Util
{
	public sealed class TextResponseValidator : AppointedResponseValidator
	{
		public TextResponseValidator(string url) : this(new Uri(url), 3)
		{
		}

		public TextResponseValidator(Uri uri) : this(uri, 3)
		{
		}

		public TextResponseValidator(string url, byte retryTimes) : this(new Uri(url), retryTimes)
		{
		}

		public TextResponseValidator(Uri uri, byte retryTimes) : base(uri, retryTimes)
		{
		}

		public static readonly TextResponseValidator BaiduHttp = new TextResponseValidator("http://www.baidu.com")
		{
			Lastword = "</html>",
			Keyword = "baidu",
			KeywordComparisonType = StringComparison.CurrentCultureIgnoreCase
		};

		public static readonly TextResponseValidator BaiduHttps = new TextResponseValidator("https://www.baidu.com")
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

		public override bool CheckPass(string response)
		{
			if (!base.CheckPass(response))
			{
				return false;
			}
			if (!string.IsNullOrEmpty(Lastword) && !response.EndsWith(Lastword, LastwordComparisonType))
			{
				return false;
			}
			if (!string.IsNullOrEmpty(Firstword) && !response.StartsWith(Firstword, FirstwordComparisonType))
			{
				return false;
			}
			if (!string.IsNullOrEmpty(Keyword) && !response.Contains(Keyword, KeywordComparisonType))
			{
				return false;
			}
			return true;
		}
	}
}