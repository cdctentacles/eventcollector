using CDC.EventCollector;
using Xunit;

namespace eventcollector.tests
{
    // Properties for SlidingWindowQueueTest :
    // 1. We can store transactions and get them all.
    // 2. If we store transaction with lsn-1, it will not come in query for lsn < lsn-1.
    // 3. If we store transaction with lsn-1, it will come in query for lsn == lsn-1.
    // 4. If we store transaction with lsn-1, it will come in query for lsn > lsn-1.
    // 5. Sliding the window till lsn-1 removes all transactions till lsn-1 including it.
    //    It should not be visible in query from then.
    // 6. All above points [1-5] should work in multi-threaded environment.
    public class SlidingWindowQueueTest
    {
        [Fact]
        public void Test1()
        {
        }
    }
}