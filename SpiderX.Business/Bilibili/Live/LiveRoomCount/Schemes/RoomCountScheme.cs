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
				int liveRoomCount = await Collector.CollectRoomCountAsync("0").ConfigureAwait(true);
				ShowLogInfo(liveRoomCount.ToString());
				var item = BilibiliLiveRoomCount.Create(liveRoomCount);
				using var context = new BilibiliDbContext(DbConfig);
				context.Database.EnsureCreated();
				context.LiveRoomCount.Add(item);
				await context.SaveChangesAsync();
			}
		}
	}
}