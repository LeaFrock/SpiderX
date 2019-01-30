using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SpiderX.Extensions.Http
{
	public static class HttpExtension
	{
		private const string HtmlCharsetPrefix = "charset=";

		public async static Task<string> ToHtmlTextAsync(this HttpResponseMessage responseMessage)
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
			MemoryStream copyStream = new MemoryStream();
			source.CopyTo(copyStream);
			/* After copying, put the pointer back to the beginning.
			 * Otherwise the StreamReader.ReadToEnd() will always return string.Empty.
			 */
			copyStream.Seek(0, SeekOrigin.Begin);
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

		public static string ToText(this HttpWebResponse response)
		{
			Stream responseStream = response.GetResponseStream();
			//Check Encoding
			Encoding encoding;
			if (response.CharacterSet != null)
			{
				try
				{
					encoding = Encoding.GetEncoding(response.CharacterSet);
				}
				catch (ArgumentException)
				{
					encoding = Encoding.UTF8;
					Console.WriteLine("Invalid Encoding: " + response.CharacterSet);
				}
			}
			else
			{
				encoding = Encoding.UTF8;
			}
			//Check GZip or Deflate
			Stream readStream;
			if (response.ContentEncoding != null)
			{
				string contentEncoding = response.ContentEncoding.ToLower();
				if (contentEncoding.Contains("gzip"))
				{
					readStream = new GZipStream(responseStream, CompressionMode.Decompress);
				}
				else if (contentEncoding.Contains("deflate"))
				{
					readStream = new DeflateStream(responseStream, CompressionMode.Decompress);
				}
				else
				{
					readStream = responseStream;
				}
			}
			else
			{
				readStream = responseStream;
			}
			//Output Text
			using (StreamReader sr = new StreamReader(readStream, encoding))
			{
				return sr.ReadToEnd();
			}
		}

		private static StreamReader CreateHtmlReaderByCharset(Stream stream, string charSet)
		{
			Encoding encoding = null;
			if (string.IsNullOrEmpty(charSet))
			{
				var ms = new MemoryStream();
				stream.CopyTo(ms);
				stream.Seek(0, SeekOrigin.Begin);
				ms.Seek(0, SeekOrigin.Begin);
				using (var tempReader = new StreamReader(stream))
				{
					bool existsCharset = false;
					int index;
					string line;
					while ((line = tempReader.ReadLine()) != null)
					{
						index = line.IndexOf(HtmlCharsetPrefix, StringComparison.CurrentCultureIgnoreCase);
						if (index >= 0)
						{
							int nextCharIndex = index + HtmlCharsetPrefix.Length;
							char nextChar = line[nextCharIndex];
							if (nextChar == '"')//Such as "<meta charset="gb2312">"
							{
								int lastQuotIndex = line.IndexOf('"', nextCharIndex + 1);
								if (lastQuotIndex < 0)
								{
									encoding = Encoding.UTF8;
								}
								else
								{
									charSet = line.Substring(nextCharIndex + 1, lastQuotIndex - nextCharIndex - 1);
								}
							}
							else//Such as "<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />"
							{
								int lastQuotIndex = line.IndexOf('"', nextCharIndex);
								if (lastQuotIndex < 0)
								{
									encoding = Encoding.UTF8;
								}
								else
								{
									charSet = line.Substring(nextCharIndex, lastQuotIndex - nextCharIndex);
								}
							}
							existsCharset = true;
							break;
						}
					}
					if (!existsCharset)
					{
						encoding = Encoding.UTF8;
					}
				}
				stream = ms;
			}
			if (encoding == null)
			{
				try
				{
					encoding = Encoding.GetEncoding(charSet);
				}
				catch
				{
#if DEBUG
					Debug.WriteLine($"{nameof(ToHtmlReaderAsync)}: GetEncoding[{charSet}]_Fail.");
#endif
					encoding = Encoding.UTF8;
				}
			}
			return new StreamReader(stream, encoding);
		}
	}
}