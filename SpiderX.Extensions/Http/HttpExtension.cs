using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SpiderX.Extensions.IO;

namespace SpiderX.Extensions.Http
{
	public static class HttpExtension
	{
		private const string HtmlCharsetPrefix = "charset=";

		private readonly static IReadOnlyDictionary<string, string> _charsetFixDict = new Dictionary<string, string>()
		{
			{ string.Empty, "utf-8" },
			{ "utf8", "utf-8" }
		};

		public async static Task<string> ToTextAsync(this HttpResponseMessage responseMessage)
		{
			var content = responseMessage.Content;
			EnsureHttpContentHeadersValid(content.Headers);
			string text = await content.ReadAsStringAsync();
			return text?.Trim();
		}

		public async static Task<Stream> ToStreamAsync(this HttpContent content)
		{
			Stream source = await content.ReadAsStreamAsync();
			/* Because HttpRequestMessage may be disposed by any task/thread in an async-environment,
			 * and the stream from ReadAsStreamAsync() will be disposed at the same time,
			 * the source-stream can't be ensured when to be disposed
			 * and must be copied(MemoryStream is a good choice).
			 * Otherwise ReadingStreamException may occur for the close of source-stream.
			 */
			MemoryStream copyStream = source.CloneNew<MemoryStream>(false);
			foreach (string s in content.Headers.ContentEncoding)
			{
				if (s.Equals("gzip", StringComparison.CurrentCultureIgnoreCase))
				{
					return new GZipStream(copyStream, CompressionMode.Decompress);
				}
				if (s.Equals("deflate", StringComparison.CurrentCultureIgnoreCase))
				{
					return new DeflateStream(copyStream, CompressionMode.Decompress);
				}
			}
			return copyStream;
		}

		#region Html

		public async static Task<StreamReader> ToHtmlReaderAsync(this HttpContent content)
		{
			Stream stream = await content.ToStreamAsync();
			string charSet = content.Headers.ContentType?.CharSet;
			return CreateHtmlReaderByCharset(stream, charSet);
		}

		private static StreamReader CreateHtmlReaderByCharset(Stream stream, string charSet)
		{
			if (!string.IsNullOrEmpty(charSet))
			{
				var e = GetEncodingByCharset(charSet);
				return new StreamReader(stream, e);
			}
			Encoding encoding = Encoding.UTF8;
			using (var tempReader = new StreamReader(stream))
			{
				int index;
				string line;
				while ((line = tempReader.ReadLine()) != null)
				{
					index = line.IndexOf(HtmlCharsetPrefix, StringComparison.CurrentCultureIgnoreCase);
					if (index >= 0)
					{
						int nextCharIndex = index + HtmlCharsetPrefix.Length;
						int firstQuotIndex = line.IndexOf('"', nextCharIndex);
						if (firstQuotIndex >= 0)
						{
							if (firstQuotIndex <= nextCharIndex + 2)//Such as "<meta charset="gb2312">"
							{
								int nextQuotIndex = line.IndexOf('"', nextCharIndex + 3);
								if (nextQuotIndex >= 0)
								{
									charSet = line.Substring(firstQuotIndex + 1, nextQuotIndex - firstQuotIndex - 1);
									encoding = GetEncodingByCharset(charSet);
								}
							}
							else//Such as "<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />"
							{
								charSet = line.Substring(nextCharIndex, firstQuotIndex - nextCharIndex);
								encoding = GetEncodingByCharset(charSet);
							}
						}
						break;
					}
				}
				stream.Seek(0, SeekOrigin.Begin);
				var ms = stream.CloneNew<MemoryStream>(false);
				return new StreamReader(ms, encoding);
			}
		}

		#endregion Html

		private static Encoding GetEncodingByCharset(string charSet)
		{
			try
			{
				return Encoding.GetEncoding(charSet);
			}
			catch
			{
#if DEBUG
				Debug.WriteLine(nameof(GetEncodingByCharset) + ':' + charSet);
#endif
				return Encoding.UTF8;
			}
		}

		private static void EnsureHttpContentHeadersValid(HttpContentHeaders headers)
		{
			var contentType = headers.ContentType;
			if (contentType != null)
			{
				string charSet = contentType.CharSet;
				if (charSet != null)
				{
					if (_charsetFixDict.TryGetValue(charSet.Trim(), out string correctCharSet))
					{
						contentType.CharSet = correctCharSet;
					}
				}
			}
		}
	}
}