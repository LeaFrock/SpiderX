using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;
using SpiderX.Business.LaGou.DbEntities;
using SpiderX.Extensions;
using SpiderX.Http;
using SpiderX.Tools;

namespace SpiderX.Business.LaGou
{
	public sealed partial class LaGouBll
	{
		private static class PcWebApiProvider
		{
			public const string HomePageHost = "www.lagou.com";
			public const string HomePageUrl = "https://www.lagou.com/";

			#region PositionAjax

			public static Uri GetPositionAjaxUri(string cityName, string type = "new")
			{
				string urlString = $"https://www.lagou.com/jobs/positionAjax.json?px={type}&gx=全职&city={cityName}&needAddtionalResult=false";
				return new Uri(urlString);
			}

			//public static Uri GetPositionAjaxRefererUri(string cityName, string keyword, string type = "new")
			//{
			//	return GetJobsListUri(cityName, keyword, type);
			//}

			public static HttpContent GetPositionAjaxFormData(string keyword, string pageNum)
			{
				KeyValuePair<string, string>[] pairs = new KeyValuePair<string, string>[]
				{
					new KeyValuePair<string, string>("first", "false"),
					new KeyValuePair<string, string>("pn", pageNum),
					new KeyValuePair<string, string>("kd", keyword),
				};
				var content = new FormUrlEncodedContent(pairs);
				content.Headers.ContentType = HttpConsole.GetContentType("application/x-www-form-urlencoded;UTF-8");
				return content;
			}

			public static LaGouResponseData CreateResponseData(string response)
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
				LaGouResponseData result = new LaGouResponseData(positions.Count);
				var hrInfoMap = content.Value<JToken>("hrInfoMap")?.ToObject<Dictionary<string, JToken>>();
				if (!hrInfoMap.IsNullOrEmpty())
				{
					foreach (var pos in positions)
					{
						var responseItem = CreateResponseItem(pos);
						if (responseItem != null)
						{
							if (responseItem.Position != null)
							{
								if (hrInfoMap.TryGetValue(responseItem.Position.PositionId.ToString(), out var temp))
								{
									FillHrInfoEntity(responseItem.HrInfo, (JObject)temp);
								}
							}
							result.AddResponseItem(responseItem);
						}
					}
				}
				else
				{
					foreach (var pos in positions)
					{
						var responseItem = CreateResponseItem(pos);
						if (responseItem != null)
						{
							result.AddResponseItem(responseItem);
						}
					}
				}
				return result;
			}

			private static LaGouResponseItem CreateResponseItem(JToken jsonItem)
			{
				//Position
				long positionId = jsonItem.Value<long>("positionId");
				string positionName = jsonItem.Value<string>("positionName");
				string firstType = jsonItem.Value<string>("firstType");
				string secondType = jsonItem.Value<string>("secondType");
				string thirdType = jsonItem.Value<string>("thirdType");
				string education = jsonItem.Value<string>("education");
				string positionAdvantage = jsonItem.Value<string>("positionAdvantage");
				DateTime createTime = jsonItem.Value<DateTime>("createTime");
				string salaryText = jsonItem.Value<string>("salary");
				(int minSalary, int maxSalary) = GetRangeFromText(salaryText);
				string workYearText = jsonItem.Value<string>("workYear");
				(int minWorkYear, int maxWorkYear) = GetRangeFromText(workYearText);
				//Company
				int companyId = jsonItem.Value<int>("companyId");
				string companyName = jsonItem.Value<string>("companyShortName");
				string companyFullName = jsonItem.Value<string>("companyFullName");
				string industryField = jsonItem.Value<string>("industryField");
				string financeStage = jsonItem.Value<string>("financeStage");
				string districtName = jsonItem.Value<string>("district");
				string subwayLine = jsonItem.Value<string>("subwayline");
				string stationName = jsonItem.Value<string>("stationname");
				double lat = jsonItem.Value<double>("latitude");
				double lng = jsonItem.Value<double>("longitude");
				string labelText = string.Empty;
				JArray labels = jsonItem.Value<JArray>("companyLabelList");
				if (!labels.IsNullOrEmpty())
				{
					StringBuilder sb = new StringBuilder(labels.Count);
					sb.Append(labels[0]);
					for (byte i = 1; i < labels.Count; i++)
					{
						sb.Append(',');
						sb.Append(labels[i]);
					}
					labelText = sb.ToString();
				}
				JArray zones = jsonItem.Value<JArray>("businessZones");
				string zoneName = zones.IsNullOrEmpty() ? string.Empty : (zones[0]?.ToString() ?? string.Empty);
				string companySizeText = jsonItem.Value<string>("companySize");
				int minCompanySize = 0, maxCompanySize = 0;
				if (!string.IsNullOrWhiteSpace(companySizeText))
				{
					var companySizes = StringTool.MatchIntList(companySizeText, true);
					switch (companySizes.Count)
					{
						case 0:
							break;

						case 1:
							if (companySizeText.Contains("少于"))
							{
								maxCompanySize = companySizes[0];
							}
							else
							{
								minCompanySize = companySizes[0];
							}
							break;

						default:
							minCompanySize = companySizes[0];
							maxCompanySize = companySizes[1];
							break;
					}
				}
				//Publisher Info
				long publisherId = jsonItem.Value<long>("publisherId");
				//Publisher Record
				int resumeProcessRate = jsonItem.Value<int>("resumeProcessRate");
				int resumeProcessDay = jsonItem.Value<int>("resumeProcessDay");
				long lastloginTime = jsonItem.Value<long>("lastLogin");
				LaGouPositionEntity pe = new LaGouPositionEntity()
				{
					Name = positionName,
					CompanyId = companyId,
					PositionId = positionId,
					PublisherId = publisherId,
					Education = education,
					MinWorkYear = minWorkYear,
					MaxWorkYear = maxWorkYear,
					MinSalary = minSalary,
					MaxSalary = maxSalary,
					FirstType = firstType,
					SecondType = secondType,
					ThirdType = thirdType,
					Advantage = positionAdvantage,
					CreateTime = createTime
				};
				LaGouCompanyEntity ce = new LaGouCompanyEntity()
				{
					CompanyId = companyId,
					Name = companyName,
					FullName = companyFullName,
					MinSize = minCompanySize,
					MaxSize = maxCompanySize,
					FinanceStage = financeStage,
					IndustryField = industryField,
					LabelDescription = labelText,
					DistrictName = districtName,
					ZoneName = zoneName,
					SubwayLine = subwayLine,
					StationName = stationName,
					Latitude = lat,
					Longitude = lng
				};
				LaGouHrInfoEntity hie = new LaGouHrInfoEntity()
				{
					UserId = publisherId,
					CompanyId = companyId,
				};
				LaGouHrDailyRecordEntity hdre = new LaGouHrDailyRecordEntity()
				{
					UserId = publisherId,
					ResumeProcessDay = resumeProcessDay,
					ResumeProcessRate = resumeProcessRate,
					LastLoginTimestamp = lastloginTime,
					DateNumber = DateTime.Now.ToDateNumber()
				};
				return new LaGouResponseItem()
				{
					Company = ce,
					Position = pe,
					HrInfo = hie,
					HrDailyRecord = hdre
				};
			}

			private static void FillHrInfoEntity(LaGouHrInfoEntity target, JToken info)
			{
				long userId = info.Value<long>("userId");
				string name = info.Value<string>("realName");
				string position = info.Value<string>("positionName");
				string level = info.Value<string>("userLevel");

				target.UserId = userId;
				target.Name = name;
				target.Position = position;
				target.Level = level;
			}

			private static (int min, int max) GetRangeFromText(string text)
			{
				if (string.IsNullOrWhiteSpace(text))
				{
					return (0, 0);
				}
				var array = StringTool.MatchIntList(text, true);
				switch (array.Count)
				{
					case 0:
						return (0, 0);

					case 1:
						return (array[0], 0);

					default:
						return (array[0], array[1]);
				}
			}

			#endregion PositionAjax

			#region JobsList

			public static Uri GetJobListUri(string cityName, string keyword, string type = "new")
			{
				string encodedKeyword = WebTool.UrlEncode(keyword);
				string encodedCityName = WebTool.UrlEncode(cityName);
				string urlString = $"https://www.lagou.com/jobs/list_{encodedKeyword}?px={type}&gx=%E5%85%A8%E8%81%8C&city={encodedCityName}";
				return new Uri(urlString);
			}

			#endregion JobsList
		}
	}
}