using System;
using System.Collections.Generic;

using CDC.EventCollector;
using Xunit;

namespace eventcollector.tests
{
    public class UserScenario
    {
        [Fact]
        public void UserScenario1()
        {
            var sourceFactories = new List<ISourceFactory>();
            var persistentCollectors = new List<IPersistentCollector>();
            var conf = new Configuration(sourceFactories, persistentCollectors)
                .SetHealthStore(new TestHealthStore());
            CDCCollector.AddConfiguration(conf);
        }
    }
}
