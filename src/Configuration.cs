using System.Collections.Generic;

namespace CDC.EventCollector
{
    public class Configuration
    {
        public Configuration(List<ISourceFactory> sourceFactories, List<IPersistentCollector> collectors)
        {
            this.sourceFactories = sourceFactories;
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

        public List<ISourceFactory> SourceFactories
        {
            get { return sourceFactories; }
        }

        public List<IPersistentCollector> PersistentCollectors
        {
            get { return persistentCollectors; }
        }

        private List<ISourceFactory> sourceFactories;
        private List<IPersistentCollector> persistentCollectors;
        private IHealthStore healthStore;
    }
}