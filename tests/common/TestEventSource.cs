using System;

using CDC.EventCollector;

namespace eventcollector.tests
{
    public class TestEventSource : Source
    {
        public TestEventSource(IEventCollector collector, IHealthStore healthStore) :
        base(collector, healthStore)
        {
            this.Id = new Guid();
            this.collector = collector;
        }

        public override Guid GetSourceId()
        {
            return this.Id;
        }

        public override ITransactionalLog GetTransactionalLog()
        {
            return null;
        }

        public IEventCollector Collector
        {
            get { return this.collector; }
        }

        Guid Id;
        private IEventCollector collector;
    }
}
