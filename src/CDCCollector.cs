using System.Collections.Generic;

namespace CDC.EventCollector
{
    public static class CDCCollector
    {
        public static void NewCollectorConfiguration(Configuration configuration)
        {
            var collector = new EventCollector();
            AddConfigurationToCollector(collector, configuration);
        }

        private static void AddConfigurationToCollector(EventCollector collector, Configuration configuration)
        {
            configuration.SourceFactory.CreateSource(collector, configuration.HealthStore);
            collector.AddPersistentCollectors(configuration.PersistentCollectors);
        }
    }
}