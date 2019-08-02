using System.Net;
using System.Text.Encodings.Web;

namespace SpiderX.Tools
{
	public static class WebTool
	{
		/// <summary>
		/// For example, WHITESPACE will be encoded as '+'
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string UrlEncodeByW3C(string text)
		{
			return WebUtility.UrlEncode(text);
		}

		public static string UrlDecodeByW3C(string text)
		{
			return WebUtility.UrlDecode(text);
		}

		/// <summary>
		/// For example, WHITESPACE will be encoded as '%20'
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string UrlEncodeByRFC(string text)
		{
			return UrlEncoder.Default.Encode(text);
		}
	}
}