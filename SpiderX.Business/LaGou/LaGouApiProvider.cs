using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using SpiderX.Business.LaGou.DbEntities;
using SpiderX.Extensions;
using SpiderX.Tools;

namespace SpiderX.Business.LaGou
{
	public sealed partial class LaGouBll
	{
		private static class PcWebApiProvider
		{
			public const string HomePageHost = "www.lagou.com";
			public const string HomePageUrl = "https://www.lagou.com/";

			public static string GetRequestUrl(string cityName, string type = "new")
			{
				return $"https://www.lagou.com/jobs/positionAjax.json?px={type}&city={cityName}&needAddtionalResult=false";
			}

			public static HttpContent GetRequestFormData(string keyword, string pageNum)
			{
				KeyValuePair<string, string>[] pairs = new KeyValuePair<string, string>[]
				{
					new KeyValuePair<string, string>("first", "true"),
					new KeyValuePair<string, string>("pn", pageNum),
					new KeyValuePair<string, string>("kd", keyword),
				};
				return new FormUrlEncodedContent(pairs);
			}

			public static LaGouResponseEntity CreateResponseEntity(string response)
			{
				JToken source = JsonTool.DeserializeObject<JToken>(response);
				JToken content = source?.Value<JToken>("content");
				if (content == null)
				{
					return null;
				}
				JToken positionResult = content.Value<JToken>("positionResult");
				JArray positions = positionResult?.Value<JArray>("result");
				if (positions.IsNullOrEmpty())
				{
					return null;
				}
				LaGouResponseEntity result = new LaGouResponseEntity();
				Dictionary<long, JToken> hrInfoMap = content.Value<Dictionary<long, JToken>>("hrInfoMap");
				if (!hrInfoMap.IsNullOrEmpty())
				{
					var hrInfoDict = new Dictionary<long, LaGouHrInfoEntity>(hrInfoMap.Count);
					foreach (var hrInfo in hrInfoMap)
					{
						var e = CreateHRInfoEntity(hrInfo.Value);
						if (e != null)
						{
							hrInfoDict.Add(hrInfo.Key, e);
						}
					}
					result.HrInfos = hrInfoDict;
				}
				Dictionary<int, LaGouCompanyEntity> companyEntities = new Dictionary<int, LaGouCompanyEntity>(positions.Count);
				List<LaGouPositionEntity> positionEntities = new List<LaGouPositionEntity>(positions.Count);
				foreach (var pos in positions)
				{
				}
				return result;
			}

			private static (LaGouPositionEntity positionEntity, LaGouCompanyEntity companyEntity) CreatePositionEntity(JToken posItem)
			{
				//Position
				long positionId = posItem.Value<long>("positionId");
				string positionName = posItem.Value<string>("positionName");
				string firstType = posItem.Value<string>("firstType");
				string secondType = posItem.Value<string>("secondType");
				string thirdType = posItem.Value<string>("thirdType");
				string education = posItem.Value<string>("education");
				string positionAdvantage = posItem.Value<string>("positionAdvantage");
				DateTime createTime = posItem.Value<DateTime>("createTime");
				string salaryText = posItem.Value<string>("salary");
				if(!string.IsNullOrWhiteSpace(salaryText))
				{

				}
				string workYearText = posItem.Value<string>("workYear");
				if (!string.IsNullOrWhiteSpace(workYearText))
				{

				}
				//Company
				int companyId = posItem.Value<int>("companyId");
				string companyName = posItem.Value<string>("companyShortName");
				string companyFullName = posItem.Value<string>("companyFullName");
				string industryField = posItem.Value<string>("industryField");
				string financeStage = posItem.Value<string>("financeStage");
				string districtName = posItem.Value<string>("district");
				string subwayLine = posItem.Value<string>("subwayline");
				string stationname = posItem.Value<string>("stationname");
				double lat = posItem.Value<double>("latitude");
				double lng = posItem.Value<double>("longitude");
				string labels = posItem.Value<string>("companyLabelList");
				JArray zones = posItem.Value<JArray>("businessZones");
				string zoneName = zones.IsNullOrEmpty() ? string.Empty : (zones[0]?.ToString() ?? string.Empty);
				string companySize = posItem.Value<string>("companySize");
				//Publisher Info
				long publisherId = posItem.Value<long>("publisherId");
				int resumeProcessRate = posItem.Value<int>("resumeProcessRate");
				int resumeProcessDay = posItem.Value<int>("resumeProcessDay");
				//Publisher Record
				long lastloginTime = posItem.Value<long>("lastLogin");
				LaGouPositionEntity pe = new LaGouPositionEntity()
				{
				};
				LaGouCompanyEntity ce = new LaGouCompanyEntity()
				{
				};
				return (pe, ce);
			}

			private static LaGouHrInfoEntity CreateHRInfoEntity(JToken info)
			{
				long userId = info.Value<long>("userId");
				string name = info.Value<string>("realName");
				string position = info.Value<string>("positionName");
				string level = info.Value<string>("userLevel");
				LaGouHrInfoEntity entity = new LaGouHrInfoEntity()
				{
					UserId = userId,
					Name = name,
					Position = position,
					Level = level
				};
				return entity;
			}

			private static (int minSize, int maxSize) ConvertCompanySizeFromText(string text)
			{
				if (string.IsNullOrWhiteSpace(text))
				{
					return (0, 0);
				}
				int index = text.IndexOf('-');
				if (index > 0)
				{
					string minStr = text.Substring(0, index);
					int.TryParse(minStr, out int min);
					string maxStr = text.Substring(index + 1, text.Length - index - 1);
					int.TryParse(maxStr, out int max);
					return (min, max);
				}
				///ToDo: UnFinished
				return (0, 0);
			}
		}
	}
}