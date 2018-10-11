using System.Net;
using SpiderX.Extensions.Http;

namespace SpiderX.Http
{
	public static class HttpConsole
	{
		public const string DefaultPcUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36";
		public const string DefaultMobileUserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1";

		public static string GetResponseText(this WebRequest request, HttpStatusCode? statusCode = HttpStatusCode.OK)
		{
			if (request.GetResponse() is HttpWebResponse r)
			{
				string text = statusCode.HasValue && statusCode != r.StatusCode ? null : r.ToText();
				r.Dispose();
				return text;
			}
			return null;
		}
	}
}