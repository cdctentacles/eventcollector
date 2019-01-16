using CDC.EventCollector;
using Xunit;

namespace eventcollector.tests
{
    // Properties of EventCollector:
    // Transaction T1 is applied before Transaction T2 => T1 < T2.
    // 1. If T1 < T2, then task of T1 should complete before T2.
    // 2. If T1 < T2, then if task of T1 fails then task of T2 can't complete.
    public class EventCollectorTest
    {
        [Fact]
        public void Test1()
        {
        }
    }
}