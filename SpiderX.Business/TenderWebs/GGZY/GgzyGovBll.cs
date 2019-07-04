using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SpiderX.BusinessBase;

namespace SpiderX.Business.TenderWebs
{
	public sealed partial class GgzyGovBll : BllBase
	{
		public override Task RunAsync()
		{
			throw new NotSupportedException("Must input keywords");
		}

		public override async Task RunAsync(params string[] args)
		{
			var scheme = new DefaultScheme();
			await scheme.RunAsync(args);
		}
	}
}