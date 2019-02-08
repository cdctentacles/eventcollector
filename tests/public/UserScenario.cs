using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using CDC.EventCollector;
using Xunit;

namespace eventcollector.tests
{
    public class UserScenario
    {
        [Fact]
        public void ShowUseOfPublicApi()
        {
            var persistentCollectors = new List<IPersistentCollector>();
            var conf = new Configuration(new TestEventSourceFactory(), persistentCollectors)
                .SetHealthStore(new TestHealthStore());
            CDCCollector.NewCollectorConfiguration(conf);
        }

        [Fact]
        public async Task End2EndHappyPath()
        {
            var testSourceFactory = new TestEventSourceFactory();
            var persistentCollector = new TestPersistentCollector();
            var persistentCollectors = new List<IPersistentCollector>() { persistentCollector };
            var conf = new Configuration(testSourceFactory, persistentCollectors)
                .SetHealthStore(new TestHealthStore());

            Assert.Null(testSourceFactory.EventSource);
            CDCCollector.NewCollectorConfiguration(conf);
            Assert.NotNull(testSourceFactory.EventSource);

            var eventSource = testSourceFactory.EventSource;
            var transactionBytes = Encoding.ASCII.GetBytes("{}");
            var transactionLSN = 1;
            await eventSource.OnTransactionApplied(transactionLSN - 1, transactionLSN, transactionBytes);

            Assert.Single(persistentCollector.Changes);
            var persistedEvent = persistentCollector.Changes[0];
            Assert.Equal(eventSource.GetSourceId(), persistedEvent.PartitionId);
            Assert.Single(persistedEvent.Transactions);
            var persistedTransaction = persistedEvent.Transactions[0];
            Assert.Equal(transactionLSN, persistedTransaction.Lsn);
            Assert.Equal(transactionBytes, persistedTransaction.Data);
        }

        [Fact]
        public void MultiTransactionsEnd2End()
        {
            var testSourceFactory = new TestEventSourceFactory();
            var persistentCollector = new TestPersistentCollector();
            var persistentCollectors = new List<IPersistentCollector>() { persistentCollector };
            var conf = new Configuration(testSourceFactory, persistentCollectors)
                .SetHealthStore(new TestHealthStore());

            Assert.Null(testSourceFactory.EventSource);
            CDCCollector.NewCollectorConfiguration(conf);
            Assert.NotNull(testSourceFactory.EventSource);

            var eventSource = testSourceFactory.EventSource;
            var transactionBytes = Encoding.ASCII.GetBytes("{}");
            var transactionTasks = new List<Task>();
            var totalTaransactions = 1000;
            Task lastTask = null;
            for (var lsn = 1; lsn <= totalTaransactions; ++lsn)
            {
                var task = eventSource.OnTransactionApplied(lsn - 1, lsn, transactionBytes);
                if (task != lastTask)
                {
                    transactionTasks.Add(task);
                }
                lastTask = task;
            }

            Assert.True(transactionTasks.Count < 3, "Creating unnecessary task objects.");

            Task.WaitAll(transactionTasks.ToArray());

            Assert.True(persistentCollector.Changes.Count > 0, "Have not seen all transactions.");
            var currLsn = 1;
            foreach (var persistedEvent in persistentCollector.Changes)
            {
                Assert.Equal(eventSource.GetSourceId(), persistedEvent.PartitionId);
                Assert.True(persistedEvent.Transactions.Count > 0, "Each partition change should have some transaction.");
                foreach (var persistedTransaction in persistedEvent.Transactions)
                {
                    Assert.Equal(currLsn, persistedTransaction.Lsn);
                    Assert.Equal(transactionBytes, persistedTransaction.Data);
                    currLsn += 1;
                }
            }
            Assert.True(totalTaransactions + 1 == currLsn, $"Did not receive all transactions {totalTaransactions}. Got {currLsn}");
        }
    }
}
