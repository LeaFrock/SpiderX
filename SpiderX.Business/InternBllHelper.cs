using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using SpiderX.Http;

namespace SpiderX.Business
{
	internal static class InternBllHelper
	{
		internal static HttpRequestCounter CreateHttpRequestCounter(string name, ILogger logger = null, bool replaceLoggerIfExists = false)
		{
			if (!HttpRequestCounter.TryRegisterNew(name, out var counter))
			{
				return null;
			}
			if (logger != null && (replaceLoggerIfExists || HttpRequestCounter.Logger == null))
			{
				HttpRequestCounter.Logger = logger;
			}
			return counter;
		}
	}
}