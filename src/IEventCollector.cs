using System;
using System.Threading.Tasks;

namespace CDC.EventCollector
{
    public interface IEventCollector
    {
        Task TransactionApplied(Guid partitionId, long previousLsn, long lsn, byte [] transaction);
    }
}