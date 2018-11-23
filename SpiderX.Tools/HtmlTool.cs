using System.IO;
using HtmlAgilityPack;

namespace SpiderX.Http
{
	public static class HtmlTool
	{
		public static HtmlDocument LoadFromTextReader<T>(T reader, bool disposeReader = true) where T : TextReader
		{
			HtmlDocument document = new HtmlDocument();
			try
			{
				document.Load(reader);
			}
			catch
			{
				return null;
			}
			finally
			{
				if (disposeReader)
				{
					reader.Dispose();
				}
			}
			return document;
		}

		public static HtmlDocument LoadFromStream<T>(T stream, bool disposeStream = false) where T : Stream
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
			finally
			{
				if (disposeStream)
				{
					stream.Dispose();
				}
			}
			return document;
		}

		public static HtmlDocument LoadFromText(string text)
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
	}
}