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
        }

        public override Guid GetSourceId()
        {
            return this.Id;
        }

        public override ITransactionalLog GetTransactionalLog()
        {
            return null;
        }

        Guid Id;
    }
}
