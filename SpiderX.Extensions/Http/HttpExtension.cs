using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SpiderX.Extensions.IO;

namespace SpiderX.Extensions.Http
{
	public static class HttpExtension
	{
		private const string HtmlCharsetPrefix = "charset=";

		public async static Task<string> ToTextAsync(this HttpResponseMessage responseMessage)
		{
			var content = responseMessage.Content;
			if (content.Headers.ContentEncoding.IsNullOrEmpty())//ReadAsString directly.
			{
				return await responseMessage.Content.ReadAsStringAsync();
			}
			Stream finalStream = await content.ToStreamAsync();
			Encoding encoding = Encoding.UTF8;
			using (finalStream)
			{
				using (StreamReader sr = new StreamReader(finalStream, encoding))
				{
					return await sr.ReadToEndAsync();
				}
			}
		}

		public async static Task<StreamReader> ToHtmlReaderAsync(this HttpContent content)
		{
			Stream stream = await content.ToStreamAsync();
			string charSet = content.Headers.ContentType?.CharSet;
			return CreateHtmlReaderByCharset(stream, charSet);
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

		private static StreamReader CreateHtmlReaderByCharset(Stream stream, string charSet)
		{
			if (!string.IsNullOrEmpty(charSet))
			{
				var e = GetHtmlEncodingByCharset(charSet);
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
									encoding = GetHtmlEncodingByCharset(charSet);
								}
							}
							else//Such as "<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />"
							{
								charSet = line.Substring(nextCharIndex, firstQuotIndex - nextCharIndex);
								encoding = GetHtmlEncodingByCharset(charSet);
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

		private static Encoding GetHtmlEncodingByCharset(string charSet)
		{
			try
			{
				return Encoding.GetEncoding(charSet);
			}
			catch
			{
#if DEBUG
				Debug.WriteLine($"{nameof(GetHtmlEncodingByCharset)}: GetEncoding[{charSet}]_Fail.");
#endif
				return Encoding.UTF8;
			}
		}
	}
}