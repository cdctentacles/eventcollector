using System;

using CDC.EventCollector;

namespace eventcollector.tests
{
    public class TestEventSourceFactory : ISourceFactory
    {
        public ISource CreateSource(IEventCollector collector, IHealthStore healthStore)
        {
            if (this.testEventSource == null)
            {
                this.testEventSource = new TestEventSource(collector, healthStore);
            }

            return this.testEventSource;
        }

        public Source EventSource
        {
            get { return this.testEventSource; }
        }

        private Source testEventSource;
    }
}