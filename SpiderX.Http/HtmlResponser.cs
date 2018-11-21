using System.IO;
using System.Net;
using HtmlAgilityPack;

namespace SpiderX.Http
{
	public sealed class HtmlResponser
	{
		public HtmlDocument LoadHtml(Stream stream)
		{
			HtmlDocument document = new HtmlDocument();
			try
			{
				document.Load(stream);
			}
			catch
			{
				return null;
			}
			return document;
		}

		public HtmlDocument LoadHtml(string text)
		{
			HtmlDocument document = new HtmlDocument();
			try
			{
				document.LoadHtml(text);
			}
			catch
			{
				return null;
			}
			return document;
		}

		public HtmlDocument LoadHtml(WebResponse response)
		{
			HtmlDocument document = new HtmlDocument();
			try
			{
				using (Stream stream = response.GetResponseStream())
				{
					document.Load(stream);
					return document;
				}
			}
			catch
			{
				return null;
			}
			finally
			{
				response.Dispose();
			}
		}
	}
}