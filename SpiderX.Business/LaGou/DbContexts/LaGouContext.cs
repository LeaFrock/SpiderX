using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SpiderX.Business.LaGou.DbEntities;
using SpiderX.DataClient;

namespace SpiderX.Business.LaGou.DbContexts
{
    public abstract class LaGouContext : DbContext
    {
        public LaGouContext(DbConfig dbConfig) : base()
        {
            Config = dbConfig;
        }

        public DbConfig Config { get; }

        public DbSet<LaGouPositionEntity> Positions { get; set; }

        public DbSet<LaGouCompanyEntity> Companies { get; set; }

        public DbSet<LaGouHrInfoEntity> HrInfos { get; set; }

        public DbSet<LaGouHrDailyRecordEntity> HrDailyRecords { get; set; }
    }
}