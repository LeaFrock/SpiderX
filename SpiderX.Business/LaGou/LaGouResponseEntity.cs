using System;
using System.Collections.Generic;
using System.Text;
using SpiderX.Business.LaGou.DbEntities;

namespace SpiderX.Business.LaGou
{
	public sealed class LaGouResponseEntity
	{
		public Dictionary<int, LaGouCompanyEntity> Companies { get; set; }

		public List<LaGouPositionEntity> Positions { get; set; }

		public Dictionary<long, LaGouHrInfoEntity> HrInfos { get; set; }

		public Dictionary<long, LaGouHrDailyRecordEntity> HrDailyRecords { get; set; }
	}
}