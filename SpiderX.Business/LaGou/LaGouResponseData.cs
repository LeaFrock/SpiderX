using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpiderX.Business.LaGou.DbEntities;
using SpiderX.Extensions;

namespace SpiderX.Business.LaGou
{
	public sealed class LaGouResponseDataCollection
	{
		public LaGouResponseDataCollection()
		{
			Companies = new ConcurrentDictionary<int, LaGouCompanyEntity>();
			Positions = new ConcurrentDictionary<long, LaGouPositionEntity>();
			HrInfos = new ConcurrentDictionary<long, LaGouHrInfoEntity>();
			HrDailyRecords = new ConcurrentDictionary<long, LaGouHrDailyRecordEntity>();
		}

		public ConcurrentDictionary<int, LaGouCompanyEntity> Companies { get; }

		public ConcurrentDictionary<long, LaGouPositionEntity> Positions { get; }

		public ConcurrentDictionary<long, LaGouHrInfoEntity> HrInfos { get; }

		public ConcurrentDictionary<long, LaGouHrDailyRecordEntity> HrDailyRecords { get; }

		public void AddResponseData(LaGouResponseData data)
		{
			if(data == null)
			{
				return;
			}
			Companies.AddRange(data.Companies.Select(p => new KeyValuePair<int, LaGouCompanyEntity>(p.CompanyId, p)));
			Positions.AddRange(data.Positions.Select(p => new KeyValuePair<long, LaGouPositionEntity>(p.PositionId, p)));
			HrInfos.AddRange(data.HrInfos.Select(p => new KeyValuePair<long, LaGouHrInfoEntity>(p.UserId, p)));
			HrDailyRecords.AddRange(data.HrDailyRecords.Select(p => new KeyValuePair<long, LaGouHrDailyRecordEntity>(p.UserId, p)));
		}
	}

	public sealed class LaGouResponseData
	{
		public LaGouResponseData(int capacity = 16)
		{
			Companies = new List<LaGouCompanyEntity>(capacity);
			Positions = new List<LaGouPositionEntity>(capacity);
			HrInfos = new List<LaGouHrInfoEntity>(capacity);
			HrDailyRecords = new List<LaGouHrDailyRecordEntity>(capacity);
		}

		public List<LaGouCompanyEntity> Companies { get; }

		public List<LaGouPositionEntity> Positions { get; }

		public List<LaGouHrInfoEntity> HrInfos { get; }

		public List<LaGouHrDailyRecordEntity> HrDailyRecords { get; }

		public void AddResponseItem(LaGouResponseItem item)
		{
			if (item.Company != null)
			{
				Companies.Add(item.Company);
			}
			if (item.Position != null)
			{
				Positions.Add(item.Position);
			}
			if (item.HrInfo != null)
			{
				HrInfos.Add(item.HrInfo);
			}
			if (item.HrDailyRecord != null)
			{
				HrDailyRecords.Add(item.HrDailyRecord);
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