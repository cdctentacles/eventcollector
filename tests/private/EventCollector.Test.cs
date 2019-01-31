using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CDC.EventCollector;
using Xunit;

namespace eventcollector.tests
{
    // Properties of EventCollector:
    // Transaction T1 is applied before Transaction T2 => T1 < T2.
    // 1. If T1 < T2, then task of T1 should complete before T2.
    // 2. If T1 < T2, then if task of T1 fails then task of T2 can't complete.
    // 3. If T1 < T2, then PersistentCollector (PC) should get T1 before T2.
    // 4. PC should not receive same transaction again if it returns success for that.
    // 5. If PC returns failure for T1, then we should receive T1 again and
    //    not receive a partition-change with T only > T1.
    // 6. Failure cases should be re-tried again.
    // 7. This all should work with source sending Transactions one by one and
    //    scheduler running in background with re tries on failure cases.
    public class EventCollectorTest
    {
        [Fact]
        public void Test1()
        {
        }

        [Fact]
        // case 5
        public async Task FailedTransactionWillBeRetriedOnNewEvent()
        {
            var persistentCollector = new TestPersistentCollectorFailure();
            var persistentCollectors = new List<IPersistentCollector>() { persistentCollector };
            var eventCollector = new EventCollector(persistentCollectors);
            var partitionId = new Guid();
            var data = new byte[] {0x1, 0x2};

            persistentCollector.DontAcceptChangesNow();

            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await eventCollector.TransactionApplied(partitionId, 1, data));

            persistentCollector.AcceptChangesNow();

            await eventCollector.TransactionApplied(partitionId, 2, data);
            var changes = persistentCollector.Changes;

            Assert.True(1 == changes.Count);
            Assert.Equal(2, changes[0].Transactions.Count);
            Assert.Equal(1, changes[0].Transactions[0].Lsn);
            Assert.Equal(2, changes[0].Transactions[1].Lsn);
        }

        // What happens if we get lsn again if customer wants to retry it on failure.
    }
}
