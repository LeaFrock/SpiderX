using System;
using System.Collections.Generic;
using System.Text;
using SpiderX.Business.LaGou.DbEntities;

namespace SpiderX.Business.LaGou
{
	public sealed class LaGouResponseData
	{
		public Dictionary<int, LaGouCompanyEntity> Companies { get; set; }

		public List<LaGouPositionEntity> Positions { get; set; }

		public Dictionary<long, LaGouHrInfoEntity> HrInfos { get; set; }

		public Dictionary<long, LaGouHrDailyRecordEntity> HrDailyRecords { get; set; }
	}

	public sealed class LaGouResponseItem
	{
		public LaGouCompanyEntity Company { get; set; }

		public LaGouPositionEntity Position { get; set; }

		public LaGouHrInfoEntity HrInfo { get; set; }

		public LaGouHrDailyRecordEntity HrDailyRecord { get; set; }
	}
}