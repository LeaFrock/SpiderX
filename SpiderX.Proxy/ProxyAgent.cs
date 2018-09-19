using System.Collections.Generic;
using System.Linq;

namespace SpiderX.Proxy
{
    public static class ProxyAgent
    {
        public static List<SpiderProxyEntity> SelectProxyEntities()
        {
            using (var context = new ProxyDbContext())
            {
                return context.ProxyEntities.ToList();
            }
        }

        public static int AddProxyEntities(IEnumerable<SpiderProxyEntity> entities)
        {
            using (var context = new ProxyDbContext())
            {
                context.ProxyEntities.AddRange(entities);
                return context.SaveChanges();
            }
        }
    }
}