using System.Net;

namespace SpiderX.Http
{
	public static class HttpConsole
	{
		public const string DefaultPcUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
		public const string DefaultMobileUserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1";

		public static HttpWebRequest CreatePureRequest(string url)
		{
			return WebRequest.CreateHttp(url);
		}

		public static HttpWebRequest CreateRequest(string url)
		{
			HttpWebRequest request = WebRequest.CreateHttp(url);
			request.Accept = "application/json, text/javascript, */*; q=0.01";
			request.Connection = "keep-alive";
			request.ContentType = "application/json; encoding=utf-8";
			request.Timeout = 5000;
			request.UserAgent = "";
			return request;
		}

		public static HttpWebResponse GetResponse(WebRequest request)
		{
			return request.GetResponse() as HttpWebResponse;
		}
	}
}