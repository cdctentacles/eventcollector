using System;

using CDC.EventCollector;

namespace eventcollector.tests
{
    public class TestEventSourceFactory : ISourceFactory
    {
        public TestEventSourceFactory(Guid partitionId = new Guid())
        {
            this.partitionId = partitionId;
        }

        public ISource CreateSource(IEventCollector collector, IHealthStore healthStore)
        {
            if (this.testEventSource == null)
            {
                this.testEventSource = new TestEventSource(collector, healthStore, partitionId);
            }

            return this.testEventSource;
        }

        public TestEventSource EventSource
        {
            get { return this.testEventSource; }
        }

        private TestEventSource testEventSource;
        private Guid partitionId;
    }
}