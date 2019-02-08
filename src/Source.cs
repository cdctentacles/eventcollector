using System;
using System.Threading.Tasks;

namespace CDC.EventCollector
{
    public abstract class Source : ISource
    {
        public Source(IEventCollector eventCollector, IHealthStore healthStore)
        {
            this.eventCollector = eventCollector;
            this.healthStore = healthStore;
        }

        // return error type
        public Task OnTransactionApplied(long previousLsn, long lsn, byte [] transaction)
        {
            return this.eventCollector.TransactionApplied(GetSourceId(), previousLsn, lsn, transaction);
        }

        public abstract Guid GetSourceId();

        private IEventCollector eventCollector;
        private IHealthStore healthStore;
    }
}