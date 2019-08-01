using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderX.Business.LaGou
{
	public partial class LaGouBll
	{
		private sealed class LaGouSearchParam
		{
			public string City { get; set; }

			public string Keyword { get; set; }

			public string SearchType { get; set; }
		}
	}
}