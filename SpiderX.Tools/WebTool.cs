using System.Net;

namespace SpiderX.Tools
{
	public static class WebTool
	{
		/// <summary>
		/// Use for local web debugging tools, like Fiddler etc.
		/// </summary>
		public readonly static IWebProxy Local = new WebProxy("localhost", 8888);

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