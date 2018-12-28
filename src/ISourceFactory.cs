using System;

namespace CDC.EventCollector
{
    public interface ISourceFactory
    {
        ISource CreateSource(IEventCollector collector, IHealthStore healthStore);
    }
}
