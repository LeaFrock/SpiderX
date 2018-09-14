using System.IO;
using System.Net;
using HtmlAgilityPack;

namespace SpiderX.Http
{
    public sealed class HtmlResponser
    {
        public HtmlDocument LoadHtml(WebResponse response)
        {
            try
            {
                Stream stream = response.GetResponseStream();
                HtmlDocument document = new HtmlDocument();
                document.Load(stream);
                return document;
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