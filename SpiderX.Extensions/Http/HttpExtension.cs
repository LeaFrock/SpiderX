using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace SpiderX.Extensions.Http
{
	public static class HttpExtension
	{
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
	}
}