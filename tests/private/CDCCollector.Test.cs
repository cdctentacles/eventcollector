using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using CDC.EventCollector;
using Xunit;

namespace eventcollector.tests
{
    public class CDCCollectorTest
    {
        [Fact]
        public void SetupOfSourceAndPersistentCollector()
        {
            var testSourceFactory = new TestEventSourceFactory();
            var persistentCollector = new TestPersistentCollector();
            var persistentCollectors = new List<IPersistentCollector>() { persistentCollector };
            var conf = new Configuration(testSourceFactory, persistentCollectors)
                .SetHealthStore(new TestHealthStore());

            Assert.Null(testSourceFactory.EventSource);
            CDCCollector.NewCollectorConfiguration(conf);
            Assert.NotNull(testSourceFactory.EventSource);

            Assert.True(testSourceFactory.EventSource.Collector is EventCollector);
            var collector = testSourceFactory.EventSource.Collector as EventCollector;

            Assert.True(collector.IsRegisteredCollector(persistentCollector));
        }
    }
}