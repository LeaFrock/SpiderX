namespace SpiderX.DataClient
{
	public enum DbEnum : byte
	{
		SqlServer = 0,
		MySql = 1,

		Redis = 128,

		Unknown = 255//Used as a placeholder.
	}
}