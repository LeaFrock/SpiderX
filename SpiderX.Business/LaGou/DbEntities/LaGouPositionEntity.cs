using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderX.Business.LaGou.DbEntities
{
	public class LaGouPositionEntity
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Keyword { get; set; }

		public int CompanyId { get; set; }

		public long PositionId { get; set; }

		public long PublisherId { get; set; }

		public string Education { get; set; }

		public int MinWorkYear { get; set; }

		public int MaxWorkYear { get; set; }

		public int MinSalary { get; set; }

		public int MaxSalary { get; set; }

		public string FirstType { get; set; }

		public string SecondType { get; set; }

		public string ThirdType { get; set; }

		public string Advantage { get; set; }

		public DateTime CreateTime { get; set; }

		public DateTime DbCreateTime { get; set; }
	}
}