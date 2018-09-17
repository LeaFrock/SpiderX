using System.IO;
using System.Net;
using HtmlAgilityPack;

namespace SpiderX.Http
{
    public sealed class HtmlResponser
    {
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