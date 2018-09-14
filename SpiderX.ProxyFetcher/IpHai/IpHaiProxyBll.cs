using System.Net;
using HtmlAgilityPack;
using SpiderX.BusinessBase;
using SpiderX.Http;

namespace SpiderX.ProxyFetcher
{
    public sealed class IpHaiProxyBll : BllBase
    {
        public const string HomePageUrl = "www.iphai.com";

        public const string NgUrl = "http://www.iphai.com/free/ng";//国内高匿
        public const string NpUrl = "http://www.iphai.com/free/np";//国内普通
        public const string WgUrl = "http://www.iphai.com/free/wg";//国外高匿
        public const string WpUrl = "http://www.iphai.com/free/wp";//国外普通

        public override string ClassName => GetType().Name;

        public override void Run(params string[] objs)
        {
            throw new System.NotImplementedException();
        }

        public override void Run()
        {
            base.Run();
            var request = CreateRequest(NgUrl);
            var response = request.GetResponse();
            if (response == null)
            {
                return;
            }
            HtmlResponser receptor = new HtmlResponser();
            var htmlDocument = receptor.LoadHtml(response);
            if (htmlDocument == null)
            {
                return;
            }
            HtmlNodeCollection rows = htmlDocument.DocumentNode.SelectNodes("//table[contains(@class,'table')]/tr");
        }

        private HttpWebRequest CreateRequest(string url)
        {
            var request = WebRequest.CreateHttp(url);
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Host = HomePageUrl;
            request.UserAgent = HttpConsole.DefaultPcUserAgent;
            request.Referer = NgUrl;
            return request;
        }
    }
}