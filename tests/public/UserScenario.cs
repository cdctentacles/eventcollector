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
            var sourceFactories = new List<ISourceFactory>();
            var persistentCollectors = new List<IPersistentCollector>();
            var conf = new Configuration(sourceFactories, persistentCollectors)
                .SetHealthStore(new TestHealthStore());
            CDCCollector.AddConfiguration(conf);
        }

        [Fact]
        public async Task End2EndHappyPath()
        {
            var testSourceFactory = new TestEventSourceFactory();
            var sourceFactories = new List<ISourceFactory>{ testSourceFactory };
            var persistentCollector = new TestPersistentCollector();
            var persistentCollectors = new List<IPersistentCollector>() { persistentCollector };
            var conf = new Configuration(sourceFactories, persistentCollectors)
                .SetHealthStore(new TestHealthStore());

            Assert.Null(testSourceFactory.EventSource);
            CDCCollector.AddConfiguration(conf);
            Assert.NotNull(testSourceFactory.EventSource);

            var eventSource = testSourceFactory.EventSource;
            var transactionBytes = Encoding.ASCII.GetBytes("{}");
            var transactionLSN = 1;
            await eventSource.OnTransactionApplied(transactionLSN, transactionBytes);

            Assert.Single(persistentCollector.Changes);
            var persistedEvent = persistentCollector.Changes[0];
            Assert.Equal(eventSource.GetSourceId(), persistedEvent.PartitionId);
            Assert.Single(persistedEvent.Transactions);
            var persistedTransaction = persistedEvent.Transactions[0];
            Assert.Equal(transactionLSN, persistedTransaction.Lsn);
            Assert.Equal(transactionBytes, persistedTransaction.Data);
        }
    }
}
