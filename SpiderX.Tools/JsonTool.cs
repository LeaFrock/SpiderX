using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SpiderX.Http
{
	public sealed class JsonTool
	{
		public JToken DeserializeObject(string text, bool throwError = false)
		{
			JToken result;
			try
			{
				result = JsonConvert.DeserializeObject<JToken>(text);
			}
			catch
			{
				if (throwError)
				{
					throw;
				}
				return null;
			}
			return result;
		}

		public JToken DeserializeObject(Stream stream, Encoding encoding, bool throwError = false)
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
					catch
					{
						if (throwError)
						{
							throw;
						}
						return null;
					}
				}
			}
		}

		public JArray DeserializeArray(string text, bool throwError = false)
		{
			JArray result;
			try
			{
				result = JsonConvert.DeserializeObject<JArray>(text);
			}
			catch
			{
				if (throwError)
				{
					throw;
				}
				return null;
			}
			return result;
		}

		public JArray DeserializeArray(Stream stream, Encoding encoding)
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