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
        public async Task OnTransactionApplied(long previousLsn, long lsn, byte [] transaction)
        {
            await this.eventCollector.TransactionApplied(GetSourceId(), previousLsn, lsn, transaction);
        }

        public abstract Guid GetSourceId();

        private IEventCollector eventCollector;
        private IHealthStore healthStore;
    }
}