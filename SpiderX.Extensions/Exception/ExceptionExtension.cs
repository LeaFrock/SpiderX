using System;
using System.Text;

namespace SpiderX.Extensions
{
	public static class ExceptionExtension
	{
		public static string ToFullExceptionString(this Exception ex)
		{
			Exception tempEx = ex;
			StringBuilder sb = new StringBuilder(8);//默认是16，但是一般异常极少会嵌套这么多层
			sb.AppendLine(tempEx.ToString());
			while (tempEx.InnerException != null)
			{
				tempEx = tempEx.InnerException;
				sb.AppendLine(tempEx.ToString());
			}
			return sb.ToString();
		}
	}
}