using System.Collections.Generic;

namespace CDC.EventCollector
{
    public class Configuration
    {
        public Configuration(List<ISourceFactory> sourceFactories, List<IPersistentCollector> collectors)
        {
            this.sourceFactories = sourceFactories;
            this.collectors = collectors;
        }

        public Configuration SetHealthStore(IHealthStore healthStore)
        {
            this.healthStore = healthStore;
            return this;
        }

        public List<ISourceFactory> SourceFactories
        {
            get { return sourceFactories; }
        }

        private List<ISourceFactory> sourceFactories;
        private List<IPersistentCollector> collectors;
        private IHealthStore healthStore;
    }
}