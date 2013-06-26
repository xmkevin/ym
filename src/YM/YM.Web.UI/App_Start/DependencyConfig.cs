using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YM.Web.UI
{
    public class DependencyConfig
    {
        public static void ResolveDependencies(Funq.Container container)
        {
            //Register a external dependency-free 
            container.Register<ICacheClient>(new MemoryCacheClient());
            //Configure an alt. distributed peristed cache that survives AppDomain restarts. e.g Redis
            //container.Register<IRedisClientsManager>(c => new PooledRedisClientManager("localhost:6379"));
        }
    }
}