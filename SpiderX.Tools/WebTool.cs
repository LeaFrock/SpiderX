using System.Net;

namespace SpiderX.Tools
{
	public static class WebTool
	{
		public static string UrlEncode(string text)
		{
			return WebUtility.UrlEncode(text);
		}

		public static string UrlDecode(string text)
		{
			return WebUtility.UrlDecode(text);
		}
	}
}