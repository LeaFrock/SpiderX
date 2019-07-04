using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SpiderX.BusinessBase;

namespace SpiderX.Business.TenderWebs
{
	public sealed partial class GgzyGovBll : BllBase
	{
		public abstract class SchemeBase
		{
			public abstract CollectorBase Collector { get; }

			public abstract Task RunAsync(params string[] keywords);
		}
	}
}