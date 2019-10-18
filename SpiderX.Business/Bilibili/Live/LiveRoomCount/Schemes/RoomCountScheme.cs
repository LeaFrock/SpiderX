using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SpiderX.Http;

namespace SpiderX.Business.Bilibili
{
	public partial class BilibiliLiveBll
	{
		private sealed class RoomCountScheme : SchemeBase
		{
			public override async Task RunAsync()
			{
				//Collector.BeforeCollectAsync();
				int liveRoomCount = await Collector.CollectAsync("0").ConfigureAwait(false);
				ShowLogInfo(liveRoomCount.ToString());
				using var context = new BilibiliDbContext(DbConfig);
				context.Database.EnsureCreated();
				var item = BilibiliLiveRoomCount.Create(liveRoomCount);
				context.LiveRoomCount.Add(item);
				context.SaveChanges();
			}
		}
	}
}