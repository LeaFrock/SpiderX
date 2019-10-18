using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SpiderX.DataClient;

namespace SpiderX.Business.Bilibili
{
	public partial class BilibiliLiveBll
	{
		protected abstract class BilibiliLiveRoomCountContext : DbContext
		{
			public BilibiliLiveRoomCountContext(DbConfig config)
			{
				Config = config;
			}

			public DbConfig Config { get; }

			internal DbSet<BilibiliLiveRoomCount> LiveRoomCount { get; set; }
		}
	}
}