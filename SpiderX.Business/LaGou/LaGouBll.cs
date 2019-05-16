using System;
using System.Collections.Generic;
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

		public override void Run()
		{
			var scheme = new DefaultScheme() { Collector = new PcWebCollector() };
			scheme.Run();
		}

		public override void Run(params string[] args)
		{
			if (args.IsNullOrEmpty())
			{
				Run();
				return;
			}
			string schemeKey = args[0];
			if (string.IsNullOrEmpty(schemeKey))
			{
				Run();
				return;
			}
			var schemePair = Array.Find(_schemes, s => s.Key.Equals(schemeKey, StringComparison.CurrentCultureIgnoreCase));
			if (schemePair.Value == null)
			{
				Run();
				return;
			}
			var scheme = schemePair.Value.Invoke();
			scheme.Run();
		}
	}
}