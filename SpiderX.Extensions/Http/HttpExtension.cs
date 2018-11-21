using System;
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
		public async static Task<string> ToTextAsync(this HttpResponseMessage responseMessage)
		{
			var content = responseMessage.Content;
			if (content.Headers.ContentEncoding.IsNullOrEmpty())//ReadAsString directly.
			{
				return await responseMessage.Content.ReadAsStringAsync();
			}
			Stream finalStream = await content.ToStreamAsync();
			Encoding encoding = GetEncodingByCharset(content.Headers.ContentType.CharSet);
			using (finalStream)
			{
				using (StreamReader sr = new StreamReader(finalStream, encoding))
				{
					return await sr.ReadToEndAsync();
				}
			}
		}

		public async static Task<Stream> ToStreamAsync(this HttpResponseMessage responseMessage)
		{
			return await responseMessage.Content.ToStreamAsync();
		}

		public async static Task<Stream> ToStreamAsync(this HttpContent content)
		{
			Stream source = await content.ReadAsStreamAsync();
			foreach (string s in content.Headers.ContentEncoding)
			{
				if (s.Equals("gzip", StringComparison.CurrentCultureIgnoreCase))
				{
					return new GZipStream(source, CompressionMode.Decompress);
				}
				if (s.Equals("deflate", StringComparison.CurrentCultureIgnoreCase))
				{
					return new DeflateStream(source, CompressionMode.Decompress);
				}
			}
			return source;
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

		private static Encoding GetEncodingByCharset(string charset)
		{
			if (!string.IsNullOrEmpty(charset))
			{
				try
				{
					return Encoding.GetEncoding(charset);
				}
				catch
				{
					return Encoding.UTF8;
				}
			}
			return Encoding.UTF8;
		}
	}
}