using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SpiderX.Business.LaGou.DbContexts;
using SpiderX.BusinessBase;
using SpiderX.Extensions;
using SpiderX.Tools;

namespace SpiderX.Business.LaGou
{
	public sealed partial class LaGouBll : BllBase
	{
		private readonly KeyValuePair<string, Func<SchemeBase>>[] _schemeFactories = new KeyValuePair<string, Func<SchemeBase>>[]
		{
			new KeyValuePair<string, Func<SchemeBase>>("default", () => new DefaultScheme())
		};

		public LaGouBll(ILogger logger, string[] runSetting, int version) : base(logger, runSetting, version)
		{
		}

		public override async Task RunAsync()
		{
			var args = RunSettings;
			string schemeKey = args[0];
			if (!args.IsNullOrEmpty() && !string.IsNullOrEmpty(schemeKey))
			{
				var schemePair = Array.Find(_schemeFactories, s => s.Key.Equals(schemeKey, StringComparison.CurrentCultureIgnoreCase));
				if (schemePair.Value != null)
				{
					var scheme = schemePair.Value.Invoke();
					await scheme.RunAsync();
					return;
				}
			}
			var scheme0 = new DefaultScheme() { Collector = new PcWebCollector() };
			await scheme0.RunAsync();
		}
	}
}