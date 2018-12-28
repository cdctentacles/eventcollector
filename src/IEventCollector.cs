using System;

namespace CDC.EventCollector
{
    public interface IEventCollector
    {
        void TransactionApplied(Guid partitionId, long lsn, byte [] transaction);
    }
}