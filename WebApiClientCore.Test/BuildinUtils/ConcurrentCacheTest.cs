﻿using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace WebApiClientCore.Test.ResponseCaches
{
    public class ConcurrentCacheTest
    {
        private int count = 0;

        [Fact]
        public void GetOrAddGet()
        {
            var key = "WebApiClient";
            var cache = new ConcurrentCache<string, int>();

            Parallel.For(0, 1000, (i) =>
            {
                var value = cache.GetOrAdd(key, k =>
                {
                    Interlocked.Increment(ref this.count);
                    return 1;
                });

                Assert.True(value == 1);
                Assert.True(count == 1);
            });
        }
    }
}
