using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using CDC.EventCollector;
using Xunit;

namespace eventcollector.tests
{
    // Properties of EventCollector:
    // Transaction T1 is applied before Transaction T2 => T1 < T2.
    // 1. If T1 < T2, then task of T1 should complete before T2.
    //    Tested by EventCollectorScheduler.Test.cs
    // 2. WRONG : If T1 < T2, then if task of T1 fails then task of T2 can't complete.
    //    Reason : we should make progress.
    // 3. If T1 < T2, then PersistentCollector (PC) should get T1 before T2.
    //    Tested by EventCollectorScheduler.Test.cs
    // 4. PC should not receive same transaction again if it returns success for that.
    // 5. If PC returns failure for T1, then we should receive T1 again and
    //    not receive a partition-change with T only > T1.
    // 6. Failure cases should be re-tried again. Same as 5.
    //    No test : Automatically handled by new incoming events.
    // 7. This all should work with source sending Transactions one by one and
    //    scheduler running in background with re tries happening via new incoming events.
    public class EventCollectorTest
    {
        public EventCollectorTest()
        {
            this.Data = new byte[] {0x1, 0x2};
        }

        [Fact]
        // case 4.
        public async Task ShouldNotReceiveSameTransactionIfReturnSuccess()
        {
            var persistentCollector = new TestPersistentCollector();
            var persistentCollectors = new List<IPersistentCollector>() { persistentCollector };
            var eventcollector = new EventCollector(persistentCollectors);
            var partitionId = new Guid();

            await eventcollector.TransactionApplied(partitionId, 0, 1, this.Data);
            await eventcollector.TransactionApplied(partitionId, 1, 2, this.Data);

            Assert.Equal(2, persistentCollector.Changes.Count);
            Assert.Equal(1, persistentCollector.Changes[0].Transactions[0].Lsn);
            Assert.Equal(0, persistentCollector.Changes[0].Transactions[0].PreviousLsn);
            Assert.Equal(2, persistentCollector.Changes[1].Transactions[0].Lsn);
            Assert.Equal(1, persistentCollector.Changes[1].Transactions[0].PreviousLsn);
        }

        [Fact]
        // case 5 or 6.
        public async Task FailedTransactionWillBeRetriedOnNewEvent()
        {
            var persistentCollector = new TestPersistentCollectorFailure();
            var persistentCollectors = new List<IPersistentCollector>() { persistentCollector };
            var eventCollector = new EventCollector(persistentCollectors);
            var partitionId = new Guid();

            persistentCollector.DontAcceptChangesNow();

            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await eventCollector.TransactionApplied(partitionId, 0, 1, this.Data));

            persistentCollector.AcceptChangesNow();

            await eventCollector.TransactionApplied(partitionId, 1, 2, this.Data);
            var changes = persistentCollector.Changes;

            Assert.True(1 == changes.Count);
            Assert.Equal(2, changes[0].Transactions.Count);
            Assert.Equal(1, changes[0].Transactions[0].Lsn);
            Assert.Equal(2, changes[0].Transactions[1].Lsn);
        }

        [Fact]
        // case 7 and more :)
        public async Task RandomizedRealWorldTest()
        {
            var persistentCollector = new TestPersistentCollectorFailure();
            var persistentCollectors = new List<IPersistentCollector>() { persistentCollector };
            var eventCollector = new EventCollector(persistentCollectors);
            var partitionId = new Guid();
            var rand = new Random();
            const int MaxTimeToWaitInMs = 1000 * 20; // 20 seconds of test

            Func<Task> randomPCFailureFunc = async () => {
                var watch = Stopwatch.StartNew();
                while (watch.ElapsedMilliseconds < MaxTimeToWaitInMs)
                {
                    if (rand.Next(0, 10) < 3) // fail upload 30% time.
                    {
                        persistentCollector.DontAcceptChangesNow();
                    }
                    else
                    {
                        persistentCollector.AcceptChangesNow();
                    }
                    await Task.Delay(rand.Next(3, 20));
                }
            };

            var lsnTaskDict = new Dictionary<long, Task>();
            var lsn = 0;

            Func<Task> addEvents = async () => {
                var watch = Stopwatch.StartNew();
                while (watch.ElapsedMilliseconds < MaxTimeToWaitInMs)
                {
                    var t = eventCollector.TransactionApplied(partitionId, lsn - 1, lsn, this.Data);
                    lsnTaskDict.Add(lsn, t);
                    lsn += 1;
                    if (rand.Next(0, 10) < 4) // sleep 40% time.
                    {
                        await Task.Delay(rand.Next(0, 10));
                    }
                }
            };

            Task.WaitAll(new Task[] { randomPCFailureFunc(), addEvents() });
            persistentCollector.AcceptChangesNow();
            await eventCollector.TransactionApplied(partitionId, lsn - 1, lsn, this.Data);

            var changes = persistentCollector.Changes;
            var allTransactions = changes.SelectMany((pc) => pc.Transactions).ToList();
            var monotinicallyIncreasingLsns = allTransactions.Zip(allTransactions.Skip(1), (t1, t2) => t1.Lsn + 1 == t2.Lsn)
                .All(b => b == true);
            Assert.True(monotinicallyIncreasingLsns, "Some transaction is lost.");
            Assert.True(allTransactions[0].Lsn == 0, "First lsn is not right");
            Assert.True(allTransactions.Last().Lsn == lsn, "Last lsn is not right");
        }

        readonly byte[] Data;
    }
}
