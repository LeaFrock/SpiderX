using System.Collections.Generic;
using HtmlAgilityPack;
using SpiderX.Proxy;
using SpiderX.Tools;

namespace SpiderX.ProxyFetcher
{
	internal abstract class HtmlProxyApiProvider : ProxyApiProvider
	{
		public override List<SpiderProxyUriEntity> GetProxyEntities(string response)
		{
			var htmlDocument = HtmlTool.LoadFromText(response);
			if (htmlDocument == null)
			{
				return null;
			}
			return GetProxyEntities(htmlDocument);
		}

		public override List<SpiderProxyUriEntity> GetProxyEntities<T>(T reader)
		{
			var htmlDocument = HtmlTool.LoadFromTextReader(reader);
			if (htmlDocument == null)
			{
				return null;
			}
			return GetProxyEntities(htmlDocument);
		}

		protected abstract List<SpiderProxyUriEntity> GetProxyEntities(HtmlDocument htmlDocument);
	}
}