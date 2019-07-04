using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpiderX.Business.LaGou
{
	public sealed partial class LaGouBll
	{
		private abstract class SchemeBase
		{
			public CollectorBase Collector { get; set; }

			public abstract Task RunAsync();
		}
	}
}