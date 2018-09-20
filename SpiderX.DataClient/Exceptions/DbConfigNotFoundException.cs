using System;

namespace SpiderX.DataClient
{
	public sealed class DbConfigNotFoundException : Exception
	{
		public DbConfigNotFoundException() : this("DbConfig Not Found.")
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