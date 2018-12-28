using System;

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
        public void OnTransactionApplied(long lsn, byte [] transaction)
        {
            this.eventCollector.TransactionApplied(GetSourceId(), lsn, transaction);
        }

        public abstract ITransactionalLog GetTransactionalLog();

        public abstract Guid GetSourceId();

        private IEventCollector eventCollector;
        private IHealthStore healthStore;
        private ITransactionalLog transactionalLog;
    }
}