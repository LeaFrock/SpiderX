using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SpiderX.BusinessBase;

namespace SpiderX.Business.TenderWebs
{
	public sealed partial class GgzyGovBll : BllBase
	{
		public GgzyGovBll(ILogger logger, string[] runSetting, string dbConfigName, int version) : base(logger, runSetting, dbConfigName, version)
		{
		}

		public override async Task RunAsync()
		{
			var scheme = new DefaultScheme();
			await scheme.RunAsync(RunSettings).ConfigureAwait(false);
		}
	}
}