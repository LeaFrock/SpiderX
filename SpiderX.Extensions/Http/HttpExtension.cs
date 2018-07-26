using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace SpiderX.Extensions.Http
{
	public static class HttpExtension
	{
		public static string ReadText(this HttpWebResponse response, bool isDisposeRequired = true)
		{
			Stream responseStream = response.GetResponseStream();
			//Check Encoding
			Encoding encodingInstance;
			if (response.CharacterSet != null)
			{
				try
				{
					encodingInstance = Encoding.GetEncoding(response.CharacterSet);
				}
				catch (ArgumentException)
				{
					encodingInstance = Encoding.UTF8;
					Console.WriteLine("Invalid Encoding: " + response.CharacterSet);
				}
			}
			else
			{
				encodingInstance = Encoding.UTF8;
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
			string text;
			using (StreamReader sr = new StreamReader(readStream, encoding: encodingInstance))
			{
				text = sr.ReadToEnd();
			}
			if (isDisposeRequired)
			{
				response.Dispose();
			}
			return text;
		}
	}
}