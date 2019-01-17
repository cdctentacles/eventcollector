using CDC.EventCollector;
using Xunit;

namespace eventcollector.tests
{
    // Properties of EventCollectorScheduler
    // 1. Should not fire another event when one event has not completed.
    // 2. If event for lsn L1 is fired and
    //    a. it returns success, should not fire it again for L1.
    //    b. it returns failure, shoulf fire it again for L >= L1.
    // 3. Should reject new event with lsn less than already seen.
    public class EventCollectorSchedulerTest
    {
        [Fact]
        public void Test1()
        {
        }
    }
}