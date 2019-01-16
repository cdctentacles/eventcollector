using System.Collections.Generic;

namespace CDC.EventCollector
{
    public class Configuration
    {
        public Configuration(ISourceFactory sourceFactory, List<IPersistentCollector> collectors)
        {
            this.sourceFactory = sourceFactory;
            this.persistentCollectors = collectors;
        }

        public Configuration SetHealthStore(IHealthStore healthStore)
        {
            this.healthStore = healthStore;
            return this;
        }

        public IHealthStore HealthStore
        {
            get { return this.healthStore; }
        }

        public ISourceFactory SourceFactory
        {
            get { return this.sourceFactory; }
        }

        public List<IPersistentCollector> PersistentCollectors
        {
            get { return this.persistentCollectors; }
        }

        private ISourceFactory sourceFactory;
        private List<IPersistentCollector> persistentCollectors;
        private IHealthStore healthStore;
    }
}