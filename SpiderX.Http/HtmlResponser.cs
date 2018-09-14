using System.Net;
using HtmlAgilityPack;

namespace SpiderX.Http
{
	public sealed class HtmlResponser
	{
		public HtmlDocument LoadHtml(WebRequest request)
		{
			var response = request.GetResponseText();
			if (string.IsNullOrEmpty(response))
			{
				return null;
			}
			HtmlDocument document = new HtmlDocument();
			document.Load(response);
			return document;
		}
	}
}