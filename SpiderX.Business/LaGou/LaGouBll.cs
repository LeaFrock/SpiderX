using System;
using SpiderX.BusinessBase;
using SpiderX.Extensions;
using SpiderX.Tools;

namespace SpiderX.Business.LaGou
{
	public sealed partial class LaGouBll : BllBase
	{
		public override void Run()
		{
			var a = StringTool.MatchIntArray("dddd15.22222");
		}

		public override void Run(params string[] args)
		{
			Run();
		}
	}
}