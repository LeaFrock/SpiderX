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
		private readonly Dictionary<string, Func<SchemeBase>> _schemeFactories = new Dictionary<string, Func<SchemeBase>>()
		{
			{ "def_pcweb", () => new DefaultScheme() { Collector = new PcWebCollector() } },
			{ "def_pcweb_p", () => new DefaultScheme() { Collector = new PcWebCollector() { UseProxy = true } } },
			{ "def_pcwebpptr", () => new DefaultScheme() { Collector = new PcWebPptrCollector() } }
		};

		public LaGouBll(ILogger logger, string[] runSetting, string dbConfigName, int version) : base(logger, runSetting, dbConfigName, version)
		{
		}

		public override async Task RunAsync()
		{
			const int settingLength = 5;
			var args = RunSettings;//[SchemeKey,City,Keyword,SearchType(default or new)]
			if (args == null)
			{
				ShowLogError("RunSettings is NULL.");
				return;
			}
			if (args.Length != settingLength)
			{
				ShowLogError($"Invalid length of RunSettings: {args.Length}. The right length should be {settingLength}.");
				return;
			}
			string schemeKey = args[0];
			if (string.IsNullOrEmpty(schemeKey) || !_schemeFactories.TryGetValue(schemeKey.ToLowerInvariant(), out var schemeFactory))
			{
				ShowLogError($"Invalid RunSettings[0]: {schemeKey}.");
				return;
			}
			var scheme = schemeFactory.Invoke();
			if (scheme == null)
			{
				ShowLogError($"Scheme Invoke Error: {schemeKey}.");
				return;
			}
			for (byte i = 1; i < args.Length; i++)
			{
				if (string.IsNullOrEmpty(args[i]))
				{
					ShowLogError($"Invalid RunSettings[{i}]: {args[i]}.");
					return;
				}
			}
			var searchParam = new LaGouSearchParam()
			{
				CityName = args[1],
				Keyword = args[2],
				SearchType = args[3]
			};
			if (int.TryParse(args[4], out int maxPage))
			{
				searchParam.MaxPage = Math.Max(1, maxPage);
			}
			await scheme.RunAsync(searchParam).ConfigureAwait(false);
			return;
		}
	}
}