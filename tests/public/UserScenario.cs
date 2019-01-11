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
            // test in progress.
            var testSourceFactory = new TestEventSourceFactory();
            var sourceFactories = new List<ISourceFactory>{ testSourceFactory };
            var persistentCollectors = new List<IPersistentCollector>() { new TestPersistentCollector() };
            var conf = new Configuration(sourceFactories, persistentCollectors)
                .SetHealthStore(new TestHealthStore());

            Assert.Null(testSourceFactory.EventSource);
            CDCCollector.AddConfiguration(conf);
            Assert.NotNull(testSourceFactory.EventSource);

            var eventSource = testSourceFactory.EventSource;
            await eventSource.OnTransactionApplied(1, Encoding.ASCII.GetBytes("{}"));
        }
    }
}
