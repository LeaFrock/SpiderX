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

        public async static Task<string> ToTextAsync(this HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                return null;
            }
            var responseHeaders = responseMessage.Content.Headers;
            var contentType = responseHeaders.ContentType;
            string charset = contentType.CharSet;
            Encoding encoding;
            try
            {
                encoding = Encoding.GetEncoding(charset);
            }
            catch
            {
                encoding = Encoding.UTF8;
            }
            var contentEncoding = responseHeaders.ContentEncoding;
            if (contentEncoding.Count > 0)
            {
                Stream responseStream = await responseMessage.Content.ReadAsStreamAsync();
                Stream readStream = null;
                foreach (string s in contentEncoding)
                {
                    if (s.Equals("gzip", StringComparison.CurrentCultureIgnoreCase))
                    {
                        readStream = new GZipStream(responseStream, CompressionMode.Decompress);
                        break;
                    }
                    if (s.Equals("deflate", StringComparison.CurrentCultureIgnoreCase))
                    {
                        readStream = new DeflateStream(responseStream, CompressionMode.Decompress);
                        break;
                    }
                }
                if (readStream != null)
                {
                    using (StreamReader sr = new StreamReader(readStream))
                    {
                        return await sr.ReadToEndAsync();
                    }
                }
            }
            return await responseMessage.Content.ReadAsStringAsync();
        }
    }
}