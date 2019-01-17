using System.Collections.Generic;

namespace CDC.EventCollector
{
    public static class CDCCollector
    {
        public static void NewCollectorConfiguration(Configuration configuration)
        {
            var collector = new EventCollector(configuration.PersistentCollectors);
            configuration.SourceFactory.CreateSource(collector, configuration.HealthStore);
        }
    }
}