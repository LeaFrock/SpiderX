using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SpiderX.DataClient;

namespace SpiderX.Business.Bilibili
{
	public partial class BilibiliLiveRoomCountBll
	{
		public abstract class BilibiliLiveRoomCountContext : DbContext
		{
			public BilibiliLiveRoomCountContext(DbConfig config)
			{
				Config = config;
			}

			public DbConfig Config { get; }

			public DbSet<BilibiliLiveRoomCount> LiveRoomCount { get; set; }
		}
	}
}