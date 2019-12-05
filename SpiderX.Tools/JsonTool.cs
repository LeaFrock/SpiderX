using System;
using Newtonsoft.Json;

//using System.Text;
//using System.Text.Json;

namespace SpiderX.Tools
{
	public sealed class JsonTool
	{
		//public static T DeserializeObject<T>(string text, bool throwError = false) where T : class
		//{
		//	ReadOnlySpan<byte> bytes = Encoding.UTF8.GetBytes(text);
		//	T result;
		//	try
		//	{
		//		result = JsonSerializer.Deserialize<T>(bytes);
		//	}
		//	catch (JsonException ex)
		//	{
		//		if (throwError)
		//		{
		//			throw ex;
		//		}
		//		return null;
		//	}
		//	return result;
		//}

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
	}
}