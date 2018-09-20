using System;
using System.Text.RegularExpressions;
using SpiderX.BusinessBase;
using SpiderX.DataClient;
using SpiderX.Http;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher
{
	public abstract class ProxyBll : BllBase
	{
		private readonly static Regex _doubleRegex = new Regex(@"\d+(\.\d+)?", RegexOptions.None, TimeSpan.FromMilliseconds(500));

		protected readonly static Random _randomEvent = new Random();
		protected readonly static HtmlResponser _defaultHtmlResponser = new HtmlResponser();
		protected readonly static JsonResponser _defaultJsonResponser = new JsonResponser();

		public static bool MatchDoubleFromText(string text, out double value)
		{
			Match m = _doubleRegex.Match(text);
			if (!m.Success)
			{
				value = 0d;
				return false;
			}
			return double.TryParse(m.Value, out value);
		}

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