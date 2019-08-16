using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SpiderX.Tools
{
	public sealed class JsonTool
	{
		public static T DeserializeObject<T>(string text, bool throwError = false) where T : class
		{
			T result;
			try
			{
				result = JsonConvert.DeserializeObject<T>(text);
			}
			catch (Exception ex)
			{
				if (throwError)
				{
					throw ex;
				}
				return null;
			}
			return result;
		}

		public static JToken DeserializeObject(Stream stream, Encoding encoding, bool throwError = false)
		{
			JsonSerializer ser = JsonSerializer.CreateDefault();
			using (StreamReader sr = new StreamReader(stream, encoding))
			{
				using (JsonTextReader jtr = new JsonTextReader(sr) { CloseInput = false })
				{
					try
					{
						return ser.Deserialize<JToken>(jtr);
					}
					catch (Exception ex)
					{
						if (throwError)
						{
							throw ex;
						}
						return null;
					}
				}
			}
		}

		public static JArray DeserializeArray(Stream stream, Encoding encoding)
		{
			JArray result = new JArray();
			JsonSerializer ser = JsonSerializer.CreateDefault();
			using (StreamReader sr = new StreamReader(stream, encoding))
			{
				using (JsonTextReader jtr = new JsonTextReader(sr) { CloseInput = false, SupportMultipleContent = true })
				{
					while (jtr.Read())
					{
						try
						{
							JToken item = ser.Deserialize<JToken>(jtr);
							result.Add(item);
						}
						catch
						{
							continue;
						}
					}
				}
			}
			return result;
		}
	}
}