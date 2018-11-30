using System;

namespace CDC.EventCollector
{
    public interface ISource
    {
        // can't have constructor in interfaces.
        // ISource(EventCollector collector, IHealthStore healthStore, ITransactionalLog transactionalLog);
    }
}
