using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace SpiderX.Business.LaGou
{
	public sealed partial class LaGouBll
	{
		private static class PcWebApiProvider
		{
			public const string HomePageHost = "www.lagou.com";
			public const string HomePageUrl = "https://www.lagou.com/";

			public static string GetRequestUrl(string cityName, string type = "new")
			{
				return $"https://www.lagou.com/jobs/positionAjax.json?px={type}&city={cityName}&needAddtionalResult=false";
			}

			public static HttpContent GetRequestFormData(string keyword, string pageNum)
			{
				KeyValuePair<string, string>[] pairs = new KeyValuePair<string, string>[]
				{
					new KeyValuePair<string, string>("first", "true"),
					new KeyValuePair<string, string>("pn", pageNum),
					new KeyValuePair<string, string>("kd", keyword),
				};
				return new FormUrlEncodedContent(pairs);
			}
		}
	}
}