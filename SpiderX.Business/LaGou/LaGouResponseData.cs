using System;
using System.Collections.Generic;
using System.Text;
using SpiderX.Business.LaGou.DbEntities;

namespace SpiderX.Business.LaGou
{
	public sealed class LaGouResponseData
	{
		public LaGouResponseData(int capacity = 16)
		{
			Companies = new Dictionary<int, LaGouCompanyEntity>(capacity);
			Positions = new List<LaGouPositionEntity>(capacity);
			HrInfos = new Dictionary<long, LaGouHrInfoEntity>(capacity);
			HrDailyRecords = new Dictionary<long, LaGouHrDailyRecordEntity>(capacity);
		}

		public Dictionary<int, LaGouCompanyEntity> Companies { get; }

		public List<LaGouPositionEntity> Positions { get; }

		public Dictionary<long, LaGouHrInfoEntity> HrInfos { get; }

		public Dictionary<long, LaGouHrDailyRecordEntity> HrDailyRecords { get; }

		public void AddResponseItem(LaGouResponseItem item)
		{
			if (item.Company != null)
			{
				Companies[item.Company.CompanyId] = item.Company;
			}
			if (item.Position != null)
			{
				Positions.Add(item.Position);
			}
			if (item.HrInfo != null)
			{
				HrInfos[item.HrInfo.UserId] = item.HrInfo;
			}
			if (item.HrDailyRecord != null)
			{
				HrDailyRecords[item.HrDailyRecord.UserId] = item.HrDailyRecord;
			}
		}
	}

	public sealed class LaGouResponseItem
	{
		public LaGouCompanyEntity Company { get; set; }

		public LaGouPositionEntity Position { get; set; }

		public LaGouHrInfoEntity HrInfo { get; set; }

		public LaGouHrDailyRecordEntity HrDailyRecord { get; set; }
	}
}