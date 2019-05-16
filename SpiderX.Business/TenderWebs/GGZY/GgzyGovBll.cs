using System;
using System.Collections.Generic;
using System.Text;
using SpiderX.BusinessBase;

namespace SpiderX.Business.TenderWebs
{
	public sealed partial class GgzyGovBll : BllBase
	{
		public override void Run()
		{
			throw new NotSupportedException("Must input keywords");
		}

		public override void Run(params string[] args)
		{
			var scheme = new DefaultScheme();
			scheme.Run(args);
		}
	}
}