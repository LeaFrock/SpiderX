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
			new KeyValuePair<string, Func<SchemeBase>>("def_pcweb", () => new DefaultScheme() { Collector = new PcWebCollector() }),
			new KeyValuePair<string, Func<SchemeBase>>("def_pcweb_p", () => new DefaultScheme() { Collector = new PcWebCollector() { UseProxy = true } })
		};

		public LaGouBll(ILogger logger, string[] runSetting, int version) : base(logger, runSetting, version)
		{
		}

		public override async Task RunAsync()
		{
			const int settingLength = 4;
			var args = RunSettings;//[SchemeKey,City,Keyword,SearchType(default or new)]
			if (args == null)
			{
				ShowLogError("RunSettings is NULL.");
				return;
			}
			if (args.Length != settingLength)
			{
				ShowLogError($"Invalid length of RunSettings: {args.Length.ToString()}. The right length should be {settingLength.ToString()}.");
				return;
			}
			string schemeKey = args[0];
			if (string.IsNullOrEmpty(schemeKey))
			{
				ShowLogError($"Invalid RunSettings[0]: {schemeKey}.");
				return;
			}
			var schemePair = Array.Find(_schemeFactories, s => s.Key.Equals(schemeKey, StringComparison.CurrentCultureIgnoreCase));
			var scheme = schemePair.Value?.Invoke();
			if (scheme == null)
			{
				ShowLogError($"Scheme Invoke Error: {schemeKey}.");
				return;
			}
			for (byte i = 1; i < args.Length; i++)
			{
				if (string.IsNullOrEmpty(args[i]))
				{
					ShowLogError($"Invalid RunSettings[{i.ToString()}]: {args[i]}.");
					return;
				}
			}
			var searchParam = new LaGouSearchParam()
			{
				City = args[1],
				Keyword = args[2],
				SearchType = args[3]
			};
			await scheme.RunAsync(searchParam);
			return;
		}
	}
}