using System.Net;
using System.Net.Http;
using SpiderX.Http;

namespace SpiderX.Proxy
{
	public sealed class SimpleProxyValidator : SpiderProxyValidator
	{
		public SimpleProxyValidator(string urlString) : base(urlString)
		{
		}

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

		public string Keyword { get; set; }

		public string Lastword { get; set; }

		public override bool CheckPass(IWebProxy proxy)
		{
			var handler = new SocketsHttpHandler()
			{
				UseProxy = true,
				Proxy = proxy
			};
			using (var webClient = new SpiderWebClient(handler))
			{
				string responseText;
				for (byte i = 0; i < RetryTimes + 1; i++)
				{
					try
					{
						responseText = webClient.GetStringAsync(TargetUri).Result;
					}
					catch
					{
						continue;
					}
					if (string.IsNullOrEmpty(responseText))
					{
						continue;
					}
					if (!string.IsNullOrEmpty(Lastword) && !responseText.EndsWith(Lastword))
					{
						continue;
					}
					if (!string.IsNullOrEmpty(Firstword) && !responseText.StartsWith(Firstword))
					{
						continue;
					}
					if (!string.IsNullOrEmpty(Keyword) && !responseText.Contains(Keyword))
					{
						continue;
					}
					return true;
				}
				return false;
			}
		}
	}
}