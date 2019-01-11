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
            this.transactionalLog = this.GetTransactionalLog();
        }

        // return error type
        public async Task OnTransactionApplied(long lsn, byte [] transaction)
        {
            await this.eventCollector.TransactionApplied(GetSourceId(), lsn, transaction);
        }

        public abstract ITransactionalLog GetTransactionalLog();

        public abstract Guid GetSourceId();

        private IEventCollector eventCollector;
        private IHealthStore healthStore;
        private ITransactionalLog transactionalLog;
    }
}