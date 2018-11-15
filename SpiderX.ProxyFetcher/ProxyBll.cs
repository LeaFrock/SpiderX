using System;
using SpiderX.BusinessBase;
using SpiderX.DataClient;
using SpiderX.Http;
using SpiderX.Proxy;
using SpiderX.Tools;

namespace SpiderX.ProxyFetcher
{
    public abstract class ProxyBll : BllBase
    {
        protected readonly static HtmlResponser _defaultHtmlResponser = new HtmlResponser();
        protected readonly static JsonResponser _defaultJsonResponser = new JsonResponser();

        protected static Random RandomEvent => CommonTool.RandomEvent;

        public static ProxyAgent CreateProxyAgent()
        {
            var conf = DbClient.Default.FindConfig("SqlServerTest", true);
            if (conf == null)
            {
                throw new DbConfigNotFoundException();
            }
            return new ProxyAgent(conf);
        }
    }
}