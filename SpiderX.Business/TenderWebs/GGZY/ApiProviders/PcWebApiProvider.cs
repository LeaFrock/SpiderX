using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using SpiderX.BusinessBase;
using SpiderX.Extensions;
using SpiderX.Tools;

namespace SpiderX.Business.TenderWebs
{
    public partial class GgzyGovBll
    {
        public sealed class PcWebApiProvider : ApiProviderBase
        {
            public readonly static Uri ApiUri_GetBids = new Uri("http://deal.ggzy.gov.cn/ds/deal/dealList_find.jsp");

            public readonly static Uri ReferUri = new Uri("http://deal.ggzy.gov.cn/ds/deal/dealList.jsp");

            public static Uri[] GetDealUris(string json)
            {
                var source = JsonTool.DeserializeObject<JToken>(json);
                if (source == null)
                {
                    return Array.Empty<Uri>();
                }
                var urls = source.SelectTokens("$.data..url");
                var result = urls.Select(t => new Uri((string)t)).ToArray();
                return result;
            }

            public static int GetDealUris(string json, out Uri[] uris)
            {
                var source = JsonTool.DeserializeObject<JToken>(json);
                if (source == null)
                {
                    uris = null;
                    return 0;
                }
                int totalPage = source.Value<int>("ttlpage");
                if (totalPage < 1)
                {
                    uris = null;
                    return 0;
                }
                var urls = source.SelectTokens("$.data..url");
                uris = urls.Select(t => new Uri((string)t)).ToArray();
                return totalPage;
            }
        }
    }
}