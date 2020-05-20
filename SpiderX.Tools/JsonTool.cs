using System;
using System.Text.Json;

namespace SpiderX.Tools
{
    public static class JsonTool
    {
        public static T DeserializeObject<T>(string text, bool throwError = false) where T : class
        {
            T result;
            try
            {
                result = JsonSerializer.Deserialize<T>(text);
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

        public static T DeserializeObject<T>(ReadOnlySpan<byte> bytes, bool throwError = false) where T : class
        {
            T result;
            try
            {
                result = JsonSerializer.Deserialize<T>(bytes);
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