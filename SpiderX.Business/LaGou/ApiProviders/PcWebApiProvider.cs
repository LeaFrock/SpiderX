using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
            public static readonly Uri HomePageUri = new Uri("https://www.lagou.com/");

            #region PositionAjax

            public const string PositionAjaxUrlPrefix = "https://www.lagou.com/jobs/positionAjax.json";

            public static Uri GetPositionAjaxUri(string encodedCityName, string type = "new")
            {
                StringBuilder sb = new StringBuilder(5);
                sb.Append(PositionAjaxUrlPrefix + "?px=");
                sb.Append(type);
                sb.Append("&gx=%E5%85%A8%E8%81%8C&city=");
                sb.Append(encodedCityName);
                sb.Append("&needAddtionalResult=false");
                string url = sb.ToString();
                return new Uri(url);
            }

            public static Uri GetPostionAjaxReferer(string encodedCityName, string encodedKeyword, string type = "new")
            {
                StringBuilder sb = new StringBuilder(6);
                sb.Append("https://www.lagou.com/jobs/list_");
                sb.Append(encodedKeyword);
                sb.Append("?px=");
                sb.Append(type);
                sb.Append("&gx=%E5%85%A8%E8%81%8C&city=");
                sb.Append(encodedCityName);
                string url = sb.ToString();
                return new Uri(url);
            }

            public static HttpContent GetPositionAjaxContent(string encodedKeyword, string pageNum, string sid = null)
            {
                var pairs = new List<KeyValuePair<string, string>>(4)
                {
                    new KeyValuePair<string, string>("first", "false"),
                    new KeyValuePair<string, string>("pn", pageNum),
                    new KeyValuePair<string, string>("kd", encodedKeyword)
                };
                if (sid != null)
                {
                    pairs.Add(new KeyValuePair<string, string>("sid", sid));
                }
                var content = new FormUrlEncodedContent(pairs);
                content.Headers.ContentType = HttpConsole.GetOrAddContentType("application/x-www-form-urlencoded;UTF-8");
                return content;
            }

            public static LaGouResponseData CreateResponseData(string response, out string showId)
            {
                showId = null;
                JToken source = JsonTool.DeserializeObject<JToken>(response);
                JToken content = source?.Value<JToken>("content");
                if (content == null)
                {
                    return null;
                }
                showId = content.Value<string>("showId");
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
                                    responseItem.HrInfo = CreateHrInfoEntity(responseItem.Position.PublisherId, responseItem.Position.CompanyId, temp);
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
                double? lat = jsonItem.Value<double?>("latitude");
                double? lng = jsonItem.Value<double?>("longitude");
                string labelText = string.Empty;
                JArray labels = jsonItem.Value<JArray>("companyLabelList");
                if (!labels.IsNullOrEmpty() && (string)labels[0] != @"""")//The labels might be [""].
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
                            if (companySizeText.Contains("少于", StringComparison.Ordinal))
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
                DateTime? lastLogin = jsonItem.Value<DateTime?>("lastLogin");
                DateTime lastLoginTime = lastLogin ?? SqlDateTime.MinValue.Value;
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
                    Latitude = lat ?? 0,
                    Longitude = lng ?? 0
                };
                LaGouHrDailyRecordEntity hdre = new LaGouHrDailyRecordEntity()
                {
                    UserId = publisherId,
                    ResumeProcessDay = resumeProcessDay,
                    ResumeProcessRate = resumeProcessRate,
                    LastLoginTime = lastLoginTime,
                    DateNumber = DateTime.Now.ToDateNumber()
                };
                return new LaGouResponseItem()
                {
                    Company = ce,
                    Position = pe,
                    HrDailyRecord = hdre
                };
            }

            private static LaGouHrInfoEntity CreateHrInfoEntity(long? publisherId, int companyId, JToken info)
            {
                string name = info.Value<string>("realName");
                if (name is null)
                {
                    return null;
                }
                long? userId = publisherId ?? info.Value<long?>("userId");
                if (!userId.HasValue)
                {
                    return null;
                }
                string position = info.Value<string>("positionName");
                string level = info.Value<string>("userLevel");

                return new LaGouHrInfoEntity()
                {
                    UserId = userId.Value,
                    CompanyId = companyId,
                    Name = name,
                    Position = position,
                    Level = level,
                };
            }

            private static (int min, int max) GetRangeFromText(string text)
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return (0, 0);
                }
                var array = StringTool.MatchIntList(text, true);
                return array.Count switch
                {
                    0 => (0, 0),
                    1 => (array[0], 0),
                    _ => (array[0], array[1]),
                };
            }

            #endregion PositionAjax

            #region JobsList

            private readonly static ConcurrentDictionary<string, Uri> _jobListRefererCache = new ConcurrentDictionary<string, Uri>();

            public static Uri GetJobListUri(string encodedCityName, string encodedKeyword, string type = "new")
            {
                StringBuilder sb = new StringBuilder(6);
                sb.Append("https://www.lagou.com/jobs/list_");
                sb.Append(encodedKeyword);
                sb.Append("?px=");
                sb.Append(type);
                sb.Append("&gx=%E5%85%A8%E8%81%8C&city=");
                sb.Append(encodedCityName);
                string url = sb.ToString();
                return new Uri(url);
            }

            public static Uri GetJobListReferer(string encodedKeyword)
            {
                return _jobListRefererCache.GetOrAdd(encodedKeyword, k => new Uri(string.Intern($"https://www.lagou.com/zhaopin/{k}/?labelWords=label")));
            }

            #endregion JobsList
        }
    }
}