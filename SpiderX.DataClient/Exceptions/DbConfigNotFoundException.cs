using System;

namespace SpiderX.DataClient
{
	public sealed class DbConfigNotFoundException : Exception
	{
		public DbConfigNotFoundException() : this("Invalid ConfigName")
		{
		}

		public DbConfigNotFoundException(string message) : this(message, null)
		{
		}

		public DbConfigNotFoundException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}