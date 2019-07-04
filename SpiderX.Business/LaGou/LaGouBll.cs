using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpiderX.Business.LaGou.DbContexts;
using SpiderX.BusinessBase;
using SpiderX.Extensions;
using SpiderX.Tools;

namespace SpiderX.Business.LaGou
{
	public sealed partial class LaGouBll : BllBase
	{
		private readonly KeyValuePair<string, Func<SchemeBase>>[] _schemes = new KeyValuePair<string, Func<SchemeBase>>[]
		{
			new KeyValuePair<string, Func<SchemeBase>>("default", () => new DefaultScheme())
		};

		public override async Task RunAsync()
		{
			var scheme = new DefaultScheme() { Collector = new PcWebCollector() };
			await scheme.RunAsync();
		}

		public override async Task RunAsync(params string[] args)
		{
			if (args.IsNullOrEmpty())
			{
				await RunAsync();
				return;
			}
			string schemeKey = args[0];
			if (string.IsNullOrEmpty(schemeKey))
			{
				await RunAsync();
				return;
			}
			var schemePair = Array.Find(_schemes, s => s.Key.Equals(schemeKey, StringComparison.CurrentCultureIgnoreCase));
			if (schemePair.Value == null)
			{
				await RunAsync();
				return;
			}
			var scheme = schemePair.Value.Invoke();
			await scheme.RunAsync();
		}
	}
}