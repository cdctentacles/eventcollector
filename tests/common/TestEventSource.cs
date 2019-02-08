using System;

using CDC.EventCollector;

namespace eventcollector.tests
{
    public class TestEventSource : Source
    {
        public TestEventSource(IEventCollector collector, IHealthStore healthStore, Guid id = new Guid()) :
        base(collector, healthStore)
        {
            this.Id = id;
            this.collector = collector;
        }

        public override Guid GetSourceId()
        {
            return this.Id;
        }

        public IEventCollector Collector
        {
            get { return this.collector; }
        }

        Guid Id;
        private IEventCollector collector;
    }
}
