using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderX.Business.LaGou.DbEntities
{
	public sealed class LaGouCompanyEntity
	{
		public int Id { get; set; }

		public int CompanyId { get; set; }

		public string Name { get; set; }

		public string FullName { get; set; }

		public int MinSize { get; set; }

		public int MaxSize { get; set; }

		public string FinanceStage { get; set; }

		public string IndustryField { get; set; }

		public string LabelDescription { get; set; }

		public string CityName { get; set; }

		public string DistrictName { get; set; }

		public string ZoneName { get; set; }

		public string SubwayLine { get; set; }

		public string StationName { get; set; }

		public double Latitude { get; set; }

		public double Longitude { get; set; }

		public DateTime DbCreateTime { get; set; }

		public DateTime DbLatestUpdateTime { get; set; }
	}
}