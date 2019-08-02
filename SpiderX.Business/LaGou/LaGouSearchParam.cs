using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderX.Business.LaGou
{
	public partial class LaGouBll
	{
		private sealed class LaGouSearchParam
		{
			public string CityName { get; set; }

			public string Keyword { get; set; }

			public string SearchType { get; set; } = "new";

			public int MaxPage { get; set; } = 7;
		}
	}
}