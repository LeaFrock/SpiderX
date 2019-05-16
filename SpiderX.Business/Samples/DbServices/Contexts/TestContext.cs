using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SpiderX.Business.Samples
{
	public sealed class TestContext : DbContext
	{
		public DbSet<TestEntity> TestEntities { get; set; }
	}
}