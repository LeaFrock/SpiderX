using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderX.Business.LaGou.DbEntities
{
	public sealed class LaGouHrInfoEntity
	{
		public int Id { get; set; }

		public long UserId { get; set; }

		public int CompanyId { get; set; }

		public string Name { get; set; }

		public string Position { get; set; }

		public string Level { get; set; }
	}
}